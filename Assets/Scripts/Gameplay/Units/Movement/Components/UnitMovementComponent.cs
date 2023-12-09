using Unity.Entities;

namespace GC.Units.Movement
{
    public struct UnitMovementComponent : IComponentData
    {
        public float Speed;
        public float Time;
        public int SplineIndex;
    }
}
