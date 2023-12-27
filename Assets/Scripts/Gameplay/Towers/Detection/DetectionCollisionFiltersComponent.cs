using Unity.Entities;
using Unity.Physics;

public struct DetectionCollisionFiltersComponent : IComponentData
{
    public CollisionFilter unit;
    public CollisionFilter tower;
}
