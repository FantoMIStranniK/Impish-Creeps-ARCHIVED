using Unity.Entities;
using UnityEngine;

public class ProjectileTagAuthoring : MonoBehaviour
{
    
}

public class ProjectileTagBaker : Baker<ProjectileTagAuthoring>
{
    public override void Bake(ProjectileTagAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ProjectileTag());
    }
}
