using Unity.Collections;
using Unity.Entities;

[ChunkSerializable]
public struct UnitDeckComponent : IComponentData
{
    public int selectedUnit;
}

public struct UnitDeckElement : IBufferElementData
{
    public Entity unit;
}
