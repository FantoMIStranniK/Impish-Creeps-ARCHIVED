using Unity.Entities;
using Unity.Physics;

namespace GC.Gameplay.Towers.Detection
{
    public struct DetectionCollisionFiltersComponent : IComponentData
    {
        public CollisionFilter unit;
        public CollisionFilter tower;
    }
}
