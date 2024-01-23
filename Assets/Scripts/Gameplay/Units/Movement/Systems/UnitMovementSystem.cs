using Unity.Entities;
using Unity.Burst;
using GC.Gameplay.SplineFramework;

namespace GC.Gameplay.Units.Movement
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
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
            TryMoveUnit(ref state);
        }

        private void TryMoveUnit(ref SystemState state)
        {
            SplineContainer splineContainer;

            if (!SystemAPI.TryGetSingleton(out splineContainer))
                return;

            if (!splineContainer.IsSetUp)
                return;

            foreach (UnitMovementAspect unitMovementAspect in SystemAPI.Query<UnitMovementAspect>())
            {
                unitMovementAspect.MoveUnit(splineContainer, SystemAPI.Time.DeltaTime);
            }
        }
    }
}
