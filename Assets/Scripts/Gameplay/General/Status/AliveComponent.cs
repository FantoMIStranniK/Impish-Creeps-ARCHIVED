using Unity.Entities;

public struct AliveComponent : IComponentData
{
    public uint maxHealth;
    public uint currentHealth;
}
