using Unity.Entities;
using UnityEngine;

public class UnitTagAuthoring : MonoBehaviour
{
}

public class UnitTagBaker : Baker<UnitTagAuthoring>
{
    public override void Bake(UnitTagAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new UnitTag());
    }
}
