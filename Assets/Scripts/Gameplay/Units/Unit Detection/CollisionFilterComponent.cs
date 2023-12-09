using Unity.Entities;
using Unity.Physics.Authoring;

public struct CollisionFilterComponent : IComponentData
{
    public PhysicsCategoryTags friendly;
    public PhysicsCategoryTags enemy;
}
