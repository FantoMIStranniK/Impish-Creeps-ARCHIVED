using Unity.Entities;

namespace GC.Gameplay.Units.Movement
{
    [ChunkSerializable]
    public struct UnitDeckComponent : IComponentData
    {
        public int selectedUnit;
    }

    public struct UnitDeckElement : IBufferElementData
    {
        public Entity unit;
    }
}
