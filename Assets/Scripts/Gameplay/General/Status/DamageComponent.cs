using Unity.Entities;

public struct DamageComponent : IComponentData
{
    public int damage;
    public ushort pierce;
    //public DamageType damageType;
}
