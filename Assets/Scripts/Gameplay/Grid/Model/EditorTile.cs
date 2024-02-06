using Unity.Entities;
using Unity.Mathematics;

namespace GC.Gameplay.Grid
{
    public enum TileState
    {
        Vacant,
        Occupied,
        Unavailable,
    }

    [ChunkSerializable]
    public struct EditorTile
    {
        public TileState State;

        public int2 PositionInGrid;
        public float2 PositionInWorld;
        public float2 TileCenterPosition;

        public EditorTile(TileState state, int2 indexes, float2 position, float2 centerPosition)
        {
            State = state;
            PositionInGrid = indexes;
            PositionInWorld = position;
            TileCenterPosition = centerPosition;
        }
    }
}
