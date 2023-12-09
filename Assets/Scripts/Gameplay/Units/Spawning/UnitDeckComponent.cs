using Unity.Collections;
using Unity.Entities;

[ChunkSerializable]
public struct UnitDeckComponent : IComponentData
{
    public NativeArray<Entity> deck;
    public Entity test;
}
