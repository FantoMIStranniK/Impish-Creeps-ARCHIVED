using GC.SplineFramework;
using GC.Units.Movement;
using Unity.Burst;
using Unity.Entities;

namespace GC.Gameplay
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UnitMovementSystem))]
    [BurstCompile]
    public partial struct UnitProgressCheckingSystem : ISystem
    {
        public float GateHealth { get; private set; }

        [BurstCompile]
        private void OnCreate(ref SystemState state) { }

        [BurstCompile]
        private void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            CheckForCreeps(ref state);
        }

        [BurstCompile]
        private void CheckForCreeps(ref SystemState state)
        {
            SplineContainer splineContainer;

            if (!SystemAPI.TryGetSingleton(out splineContainer))
                return;

            var commandBuffer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach(EntityProgressAspect progressAspect in SystemAPI.Query<EntityProgressAspect>())
            {
                if (!progressAspect.EntityHasFinished(splineContainer, out Entity entity))
                    continue;

                commandBuffer.DestroyEntity(entity);
            }
        }
    }
}
