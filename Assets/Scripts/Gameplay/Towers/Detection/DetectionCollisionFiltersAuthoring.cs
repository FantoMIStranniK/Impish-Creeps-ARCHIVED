using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class DetectionCollisionFiltersAuthoring : MonoBehaviour
{
    public PhysicsCategoryTags unit;
    public PhysicsCategoryTags tower;
}

public class DetectionCollisionFiltersBaker : Baker<DetectionCollisionFiltersAuthoring>
{
    public override void Bake(DetectionCollisionFiltersAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new DetectionCollisionFiltersComponent
        {
            unit = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = authoring.unit.Value,
                GroupIndex = 0,
            },

            tower = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = authoring.tower.Value,
                GroupIndex = 0,
            },
        });
    }
}
