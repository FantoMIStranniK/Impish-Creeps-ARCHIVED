using Unity.Entities;

public struct AliveComponent : IComponentData
{
    public int maxHealth;
    public int currentHealth;
}
