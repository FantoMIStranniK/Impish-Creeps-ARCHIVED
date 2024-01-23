using UnityEngine;
using Unity.Entities;
using GC.Gameplay.Towers;

namespace GC.Gameplay
{
    public class TargetAuthoring : MonoBehaviour
    {

    }

    public class TargetBaker : Baker<TowerRadiusAuthoring>
    {
        public override void Bake(TowerRadiusAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new TargetComponent());
        }
    }
}
