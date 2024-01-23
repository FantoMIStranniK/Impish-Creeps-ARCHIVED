using Unity.Entities;
using Unity.Physics.Authoring;

namespace GC.Gameplay.Units.Collisions
{
    public struct CollisionFilterComponent : IComponentData
    {
        public PhysicsCategoryTags friendly;
        public PhysicsCategoryTags enemy;
    }
}
