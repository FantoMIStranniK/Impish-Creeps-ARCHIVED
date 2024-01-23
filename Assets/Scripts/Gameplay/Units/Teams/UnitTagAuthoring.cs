using UnityEngine;
using Unity.Entities;

namespace GC.Gameplay.Units.Teams
{
    public class UnitTagAuthoring : MonoBehaviour { }

    public class UnitTagBaker : Baker<UnitTagAuthoring>
    {
        public override void Bake(UnitTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitTag());
        }
    }
}
