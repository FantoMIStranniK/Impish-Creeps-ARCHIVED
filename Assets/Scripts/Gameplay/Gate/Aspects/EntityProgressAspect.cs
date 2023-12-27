using Unity.Entities;
using GC.SplineFramework;
using GC.Units.Movement;

namespace GC.Gameplay
{
    public readonly partial struct EntityProgressAspect : IAspect
    {
        private readonly RefRO<UnitMovementComponent> _unitMovementComponent;
        private readonly Entity _entity;

        public bool EntityHasFinished(SplineContainer splineContainer, out Entity entity)
        {
            entity = _entity;

            var splineIndex = _unitMovementComponent.ValueRO.SplineIndex;

            var splineLength = splineContainer.GetSplineByIndex(splineIndex).SplineLength;

            var timeOnSpline = _unitMovementComponent.ValueRO.Time;

            if (timeOnSpline < splineLength)
                return false;

            return true;
        }
    }
}
