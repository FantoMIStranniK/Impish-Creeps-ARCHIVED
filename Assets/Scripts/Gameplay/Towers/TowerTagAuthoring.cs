using Unity.Entities;
using UnityEngine;

public class TowerTagAuthoring : MonoBehaviour
{
}

public class TowerTagBaker : Baker<TowerTagAuthoring>
{
    public override void Bake(TowerTagAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new TowerTag());
    }
}
