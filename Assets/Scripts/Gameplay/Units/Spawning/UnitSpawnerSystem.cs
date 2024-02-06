using Unity.Entities;
using Unity.Burst;
using GC.Gameplay.Units.Movement;
using GC.Gameplay.SplineFramework;
using Unity.Transforms;
using Unity.Mathematics;

namespace GC.Gameplay.Units.Spawn
{
    [BurstCompile]
    [UpdateBefore(typeof(LateSimulationSystemGroup))]
    public partial struct UnitSpawnerSystem : ISystem
    {
        private BufferLookup<UnitDeckElement> unitDeckIndexLookup;
        public void OnCreate(ref SystemState state)
        {
            //state.World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>()
            unitDeckIndexLookup = state.GetBufferLookup<UnitDeckElement>(true);
        }

        public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            // get singleton of unit wave

            BeginSimulationEntityCommandBufferSystem.Singleton beginSimulationECBSystem;

            if (!SystemAPI.TryGetSingleton(out beginSimulationECBSystem))
                return;

            EntityCommandBuffer entityCommandBuffer = beginSimulationECBSystem.CreateCommandBuffer(state.World.Unmanaged);

            SplineContainer splineContainer;

            if (!SystemAPI.TryGetSingleton(out splineContainer))//
                return;

            DynamicBuffer<UnitQueueElement> queue;

            if (!SystemAPI.TryGetSingletonBuffer(out queue))
                return;

            if (queue.Length == 0)
                return;

            DynamicBuffer<UnitDeckElement> deck;

            if (!SystemAPI.TryGetSingletonBuffer(out deck, true))
                return;

            unitDeckIndexLookup.Update(ref state);

            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (UnitSpawnerAspect unitSpawner in SystemAPI.Query<UnitSpawnerAspect>())
            {
                unitSpawner.UpdateSpawnTimer(deltaTime);

                if (!unitSpawner.IsAbleToSpawn())
                    return;

                var unitDeckElement = deck[queue[0].deckIndex];
                queue.RemoveAt(0);

                Entity spawnedEntity = entityCommandBuffer.Instantiate(unitDeckElement.unit);//]);
                entityCommandBuffer.SetComponent(spawnedEntity, new LocalTransform
                {
                    Position = splineContainer.GetSplineByIndex(0).SplineSegments[0].StartPoint,
                    Rotation = quaternion.identity,
                    Scale = 1,
                });

                unitSpawner.SetAbilityToSpawn(false);
            }
        }
    }
}
