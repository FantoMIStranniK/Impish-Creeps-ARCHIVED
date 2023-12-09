using GC.SplineMovement;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace GC.Units.Movement
{
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

            foreach ((RefRW<UnitMovementComponent> movement, RefRW<LocalTransform> transform) in SystemAPI.Query<RefRW<UnitMovementComponent>, RefRW<LocalTransform>>())
            {
                var spline = splineContainer.GetSplineByIndex(movement.ValueRO.SplineIndex);

                var newTransform = spline.GetPointAndRotationOnSpline(movement.ValueRO.Time);

                transform.ValueRW.Position = newTransform.position;
                transform.ValueRW.Rotation = Quaternion.LookRotation(newTransform.rotation);

                movement.ValueRW.Time += SystemAPI.Time.DeltaTime * movement.ValueRO.Speed;
            }
        }
    }
}
