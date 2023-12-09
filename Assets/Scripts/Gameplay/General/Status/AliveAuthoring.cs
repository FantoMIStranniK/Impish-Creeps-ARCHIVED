using Unity.Entities;
using UnityEngine;

public class AliveAuthoring : MonoBehaviour
{
    public uint maxHealth;
    public uint currentHealth;
}

public class AliveBaker : Baker<AliveAuthoring>
{
    public override void Bake(AliveAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new AliveComponent
        {
            maxHealth = authoring.maxHealth,
            currentHealth = authoring.currentHealth,
        });
    }
}
