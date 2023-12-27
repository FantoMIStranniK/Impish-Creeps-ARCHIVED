using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using GC.SplineFramework;

namespace GC.Units.Movement
{
    public readonly partial struct UnitMovementAspect : IAspect
    {
        private readonly RefRW<UnitMovementComponent> _movementComponent;
        private readonly RefRW<LocalTransform> _transform;

        public void MoveUnit(SplineContainer splineContainer, float deltaTime)
        {
            var spline = splineContainer.GetSplineByIndex(_movementComponent.ValueRO.SplineIndex);

            var newTransform = spline.GetPointAndRotationOnSpline(_movementComponent.ValueRO.Time);

            _transform.ValueRW.Position = newTransform.position;
            _transform.ValueRW.Rotation = Quaternion.LookRotation(newTransform.rotation);

            _movementComponent.ValueRW.Time += deltaTime * _movementComponent.ValueRO.Speed;
        }
    }
}
