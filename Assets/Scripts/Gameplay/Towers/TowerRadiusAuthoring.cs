using UnityEngine;
using Unity.Entities;

namespace GC.Gameplay.Towers
{
    public class TowerRadiusAuthoring : MonoBehaviour
    {
        public float radius;
    }

    public class TowerRadiusBaker : Baker<TowerRadiusAuthoring>
    {
        public override void Bake(TowerRadiusAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TowerRadiusComponent
            {
                radius = authoring.radius,
            });
        }
    }
}
