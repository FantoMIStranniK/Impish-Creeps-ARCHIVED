using UnityEngine;
using Unity.Entities;

namespace GC.Gameplay.Status
{
    public class TemporaryAuthoring : MonoBehaviour
    {
        public float lifeTime;
    }

    public class TemporaryBaker : Baker<TemporaryAuthoring>
    {
        public override void Bake(TemporaryAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new TemporaryComponent
            {
                lifeTime = authoring.lifeTime,
            });
        }
    }
}
