using Unity.Entities;

public struct TemporaryComponent : IComponentData
{
    public float lifeTime;
    public float lifeTimeRemaning;
}
