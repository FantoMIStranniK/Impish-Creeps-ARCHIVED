using UnityEngine;
using Unity.Entities;
using Unity.Collections;

namespace GC.Gameplay.SplineFramework
{
    public class SplineContainerAuthoring : MonoBehaviour { }

    public class SplineContainerBaker : Baker<SplineContainerAuthoring>
    {
        public override void Bake(SplineContainerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new SplineContainer
            {
                Splines = new NativeArray<Spline>(),
                IsSetUp = false,
            });
        }
    }
}
