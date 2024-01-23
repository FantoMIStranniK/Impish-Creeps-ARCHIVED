using Unity.Entities;
using UnityEngine;

namespace GC.Gameplay.Towers
{
    public class TowerTagAuthoring : MonoBehaviour { }

    public class TowerTagBaker : Baker<TowerTagAuthoring>
    {
        public override void Bake(TowerTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new TowerTag());
        }
    }
}
