using Unity.Entities;
using GC.Gameplay.Units.Movement;
using GC.Gameplay.Units.UnitsQueue;

namespace GC.Gameplay.Units.Spawn
{
    [UpdateBefore(typeof(LateSimulationSystemGroup))]
    public partial struct UnitSpawnerSystem : ISystem
    {
        private BufferLookup<UnitDeckElement> unitDeckIndexLookup;

        public void OnCreate(ref SystemState state)
        {
            unitDeckIndexLookup = state.GetBufferLookup<UnitDeckElement>(true);
        }

        public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            TrySpawnUnits(ref state);
        }

        private void TrySpawnUnits(ref SystemState state)
        {
            BeginSimulationEntityCommandBufferSystem.Singleton beginSimulationECBSystem;

            if (!SystemAPI.TryGetSingleton(out beginSimulationECBSystem))
                return;

            EntityCommandBuffer entityCommandBuffer = beginSimulationECBSystem.CreateCommandBuffer(state.World.Unmanaged);

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

                if (unitDeckElement.unit == Entity.Null)
                    return;

                queue.RemoveAt(0);

                entityCommandBuffer.Instantiate(unitDeckElement.unit);

                unitSpawner.SetAbilityToSpawn(false);

            }
        }
    }
}
