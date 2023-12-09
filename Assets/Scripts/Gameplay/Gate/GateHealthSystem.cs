using GC.SplineMovement;
using GC.Units.Movement;
using Unity.Burst;
using Unity.Entities;
using UnityEngine.SceneManagement;

namespace GC.Gameplay
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct GateHealthSystem : ISystem
    {
        private void OnCreate(ref SystemState state) { }

        private void OnDestroy(ref SystemState state) { }

        private void OnUpdate(ref SystemState state)
        {
            var mapCheckSystem = state.World.GetOrCreateSystemManaged<MapCheckingSystem>();

            if (mapCheckSystem.CurrentMapType != MapType.Gameplay)
                return;

            CheckForCreeps(ref state);
        }

        private void CheckForCreeps(ref SystemState state)
        {
            var commandBuffer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var splineContainer = SystemAPI.GetSingleton<SplineContainer>();

            foreach ((RefRO<UnitMovementComponent> movementComponent, Entity entity) in SystemAPI.Query<RefRO<UnitMovementComponent>>().WithEntityAccess())
            {
                var splineIndex = movementComponent.ValueRO.SplineIndex;

                var splineLength = splineContainer.GetSplineByIndex(splineIndex).SplineLength;

                var timeOnSpline = movementComponent.ValueRO.Time;

                if (timeOnSpline < splineLength)
                    continue;

                commandBuffer.AddComponent(entity, new IsDisposingComponent());
            }
        }
    }
}
