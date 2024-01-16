using Unity.Collections;
using Unity.Entities;

[ChunkSerializable]
public struct TowerDeck : IComponentData
{
    public int selectedTower;
}

public struct TowerDeckElement : IBufferElementData
{
    public Entity tower;
}
