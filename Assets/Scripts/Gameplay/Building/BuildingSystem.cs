using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using RaycastHit = Unity.Physics.RaycastHit;
using GC.Utilities;
using GC.SceneManagement;
using GC.Gameplay.Grid;
using GC.Gameplay.Towers.Deck;

namespace GC.Gameplay.Building
{
    public partial class BuildingSystem : SystemBase
    {
        private PlayerControls controls;

        protected override void OnStartRunning()
        {
            controls.Towers.PlaceTower.canceled += PlaceTower;
            //controls.Towers.GetTower.performed += GetTower;
            //controls.Towers.CancelPlacement.performed += CancelPlacement;
            controls.Towers.Enable();
        }

        protected override void OnCreate()
        {
            controls = new PlayerControls();
        }

        protected override void OnUpdate() { }

        protected override void OnStopRunning()
        {
            controls.Towers.PlaceTower.canceled -= PlaceTower;
            //controls.Towers.GetTower.performed -= GetTower;
            //controls.Towers.CancelPlacement.performed -= CancelPlacement;
            controls.Towers.Disable();
        }

        private RaycastHit CastRay(float3 start, float3 end)
        {
            RaycastHit hit;

            var collisionWorld = GetCollisionWorld();

            RaycastInput raycastInput = new RaycastInput
            {
                Filter = new CollisionFilter()
                {
                    BelongsTo = (uint)1 << 31,
                    CollidesWith = (uint)1 << 31,
                },
                Start = start,
                End = end,
            };

            collisionWorld.CastRay(raycastInput, out hit);

            return hit;
        }

        private CollisionWorld GetCollisionWorld()
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();

            EntityQuery singletonQuery = EntityManager.CreateEntityQuery(builder);

            return singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        }

        private void TryPlaceTower(InputAction.CallbackContext callback)
        {
            if (!IsSuitableMap(MapType.Gameplay))
                return;

            /*if (!isPlacing)
                return;*/

            PlaceTower(callback);
        }

        private bool IsSuitableMap(MapType desiredMapType)
        {
            var mapCheckSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<MapCheckingSystem>();

            if (mapCheckSystem.CurrentMapType == desiredMapType)
                return true;

            return false;
        }

        private void PlaceTower(InputAction.CallbackContext callback)
        {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

            RefRW<TileGridComponent> grid;

            if (!SystemAPI.TryGetSingletonRW(out grid))
                return;

            RefRW<TowerDeck> deck;

            if (!SystemAPI.TryGetSingletonRW(out deck))
                return;

            DynamicBuffer<TowerDeckElement> deckBuffer = SystemAPI.GetSingletonBuffer<TowerDeckElement>(true);

            Vector2 mousePos = controls.Towers.MousePosition.ReadValue<Vector2>();
            float3 mouseWorldPos = SHelpers.GetMouseWorldPos(mousePos);

            RaycastHit hit = CastRay(mouseWorldPos, mouseWorldPos + (float3)Camera.main.transform.forward * 50);

            if (!grid.ValueRO.TileIsAvailable(hit.Position))
                return;

            int2 indexes;

            if (!grid.ValueRO.TryGetIndexesByWorldPosition(hit.Position, out indexes))
                return;
            
            Entity newTower = ecb.Instantiate(deckBuffer[deck.ValueRO.selectedTower].tower);

            grid.ValueRO.SetTileState(indexes, TileState.Occupied);

            float2 newTowerPos = grid.ValueRO.GetTile(indexes).CenterPosition;

            ecb.SetComponent(newTower, new LocalTransform
            {
                Position = new float3(newTowerPos.x, 0.5f, newTowerPos.y),
                Rotation = quaternion.identity,
                Scale = 1f
            });
        }
    }
}
