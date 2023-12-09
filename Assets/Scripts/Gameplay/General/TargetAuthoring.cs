using Unity.Entities;
using UnityEngine;

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
