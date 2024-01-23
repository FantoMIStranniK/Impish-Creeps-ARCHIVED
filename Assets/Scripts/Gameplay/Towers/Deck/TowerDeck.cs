using Unity.Entities;

namespace GC.Gameplay.Towers.Deck
{

    [ChunkSerializable]
    public struct TowerDeck : IComponentData
    {
        public int selectedTower;
    }

    public struct TowerDeckElement : IBufferElementData
    {
        public Entity tower;
    }
}
