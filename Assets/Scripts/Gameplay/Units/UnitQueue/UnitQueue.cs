using Unity.Entities;

namespace GC.Gameplay.Units.UnitsQueue
{
    public struct UnitQueueElement : IBufferElementData
    {
        public int deckIndex;
    }
}
