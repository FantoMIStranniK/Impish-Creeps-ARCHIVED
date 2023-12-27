using Unity.Entities;

[InternalBufferCapacity(2)]
public struct HitList : IBufferElementData
{
    public Entity entity;
}
