using Unity.Entities;
using GC.Gameplay.Units.Movement;
using GC.Gameplay.SplineFramework;

namespace GC.Gameplay.Gate
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
