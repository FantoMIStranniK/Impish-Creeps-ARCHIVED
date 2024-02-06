using Unity.Entities;
using Unity.Burst;
using GC.Gameplay.Units.Movement;
using GC.Gameplay.SplineFramework;

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

            BeginSimulationEntityCommandBufferSystem.Singleton buggerSystemSingletone;

            if (!SystemAPI.TryGetSingleton(out buggerSystemSingletone))
                return;

            EntityCommandBuffer entityCommandBuffer = buggerSystemSingletone.CreateCommandBuffer(state.World.Unmanaged);

            SplineContainer splineContainer;

            if (!SystemAPI.TryGetSingleton(out splineContainer))
                return;

            UnitDeckComponent unitDeck;

            if (!SystemAPI.TryGetSingleton(out unitDeck))
                return;

            unitDeckIndexLookup.Update(ref state);

            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (UnitSpawnerAspect unitSpawner in SystemAPI.Query<UnitSpawnerAspect>())
            {
                unitSpawner.UpdateSpawnTimer(deltaTime);
                unitSpawner.Spawn(entityCommandBuffer, splineContainer, SystemAPI.GetSingletonBuffer<UnitDeckElement>(true), 0);
            }
        }
    }
}
