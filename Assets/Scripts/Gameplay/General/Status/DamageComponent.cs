using Unity.Entities;

public struct DamageComponent : IComponentData
{
    public uint damage;
    public ushort pierce;
    //public DamageType damageType;
}
