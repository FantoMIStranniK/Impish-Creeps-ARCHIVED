using UnityEngine;
using Unity.Entities;

namespace GC.Gameplay.Towers.Projectiles
{
    public class ProjectileTagAuthoring : MonoBehaviour { }

    public class ProjectileTagBaker : Baker<ProjectileTagAuthoring>
    {
        public override void Bake(ProjectileTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ProjectileTag());
        }
    }
}
