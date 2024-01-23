using UnityEngine;
using Unity.Entities;

namespace GC.Gameplay.Status
{

    public class MotionAuthoring : MonoBehaviour
    {
        public float speed;
    }

    public class MotionBaker : Baker<MotionAuthoring>
    {
        public override void Bake(MotionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MotionComponent
            {
                speed = authoring.speed,
            });
        }
    }
}
