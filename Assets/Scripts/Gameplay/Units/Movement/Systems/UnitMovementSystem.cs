using GC.SplineFramework;
using Unity.Burst;
using Unity.Entities;

namespace GC.Units.Movement
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SplineInitializationSystem))]
    [BurstCompile]
    public partial struct UnitMovementSystem : ISystem
    {
        [BurstCompile]
        private void OnCreate(ref SystemState state) { }

        [BurstCompile]
        private void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        private void OnUpdate(ref SystemState state)
        {
            SplineContainer splineContainer;

            if (!SystemAPI.TryGetSingleton(out splineContainer))
                return;

            if (!splineContainer.IsSetUp)
                return;

            foreach(UnitMovementAspect unitMovementAspect in SystemAPI.Query<UnitMovementAspect>())
            {
                unitMovementAspect.MoveUnit(splineContainer, SystemAPI.Time.DeltaTime);
            }
        }
    }
}
