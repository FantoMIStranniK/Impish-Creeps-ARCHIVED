using GC.Gameplay.Towers.Deck;
using GC.Utilities;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine.InputSystem;
using UnityEngine;
using GC.Gameplay.Grid;
using GC.SceneManagement;

public partial class BuildingSystem : SystemBase
{
    private Camera camera;
    private PlayerControls controls;
    private CollisionFilter raycastInputFilter;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        controls.Towers.PlaceTower.canceled += PlaceTower;
        //controls.Towers.GetTower.performed += GetTower;
        //controls.Towers.CancelPlacement.performed += CancelPlacement;
        controls.Towers.Enable();
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        camera = Camera.main;
        controls = new PlayerControls();

        raycastInputFilter = new CollisionFilter()
        {
            BelongsTo = (uint)1 << 31,
            CollidesWith = (uint)1 << 31,
        };
    }

    protected override void OnUpdate() {}

    protected override void OnStopRunning()
    {
        base.OnStopRunning();

        controls.Towers.PlaceTower.canceled -= PlaceTower;
        //controls.Towers.GetTower.performed -= GetTower;
        //controls.Towers.CancelPlacement.performed -= CancelPlacement;
        controls.Towers.Disable();
    }

    private Unity.Physics.RaycastHit CastRay(float3 start, float3 end)
    {
        Unity.Physics.RaycastHit hit;

        EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();
        EntityQuery singletonQuery = EntityManager.CreateEntityQuery(builder);
        CollisionWorld collisionWorld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

        RaycastInput raycastInput = new RaycastInput
        {
            Filter = raycastInputFilter,
            Start = start,
            End = end,
        };

        collisionWorld.CastRay(raycastInput, out hit);

        return hit;
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

        RefRW<TileGridComponent> grid = SystemAPI.GetSingletonRW<TileGridComponent>();
        RefRW<TowerDeck> deck = SystemAPI.GetSingletonRW<TowerDeck>();
        DynamicBuffer<TowerDeckElement> deckBuffer = SystemAPI.GetSingletonBuffer<TowerDeckElement>(true);

        Vector2 mousePos = controls.Towers.MousePosition.ReadValue<Vector2>();
        float3 mouseWorldPos = SHelpers.GetMouseWorldPos(mousePos);

        Unity.Physics.RaycastHit hit = CastRay(mouseWorldPos, mouseWorldPos + ((float3)camera.transform.forward * 50));

        if (grid.ValueRO.TileIsAvailable(hit.Position))
            return;

        Entity newTower = ecb.Instantiate(deckBuffer[deck.ValueRO.selectedTower].tower);

        grid.ValueRO.SetTileState(hit.Position, TileState.Occupied);

        float2 newTowerPos = grid.ValueRO.GetTile(hit.Position).CenterPosition;

        ecb.SetComponent(newTower, new LocalTransform
        {
            Position = new float3(newTowerPos.x, 0.5f, newTowerPos.y),
            Rotation = quaternion.identity,
            Scale = 1f
        });
    }
}
