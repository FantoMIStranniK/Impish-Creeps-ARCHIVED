using UnityEngine;
using Unity.Entities;
using Unity.Physics.Authoring;

namespace GC.Gameplay.Units.Collisions
{
    public class CollisionFilterAuthoring : MonoBehaviour
    {
        public PhysicsCategoryTags friendly;
        public PhysicsCategoryTags enemy;
    }

    public class CollisionFilterBaker : Baker<CollisionFilterAuthoring>
    {
        public override void Bake(CollisionFilterAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new CollisionFilterComponent
            {
                friendly = authoring.friendly,
                enemy = authoring.enemy,
            });
        }
    }
}
