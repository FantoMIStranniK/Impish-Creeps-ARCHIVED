using Unity.Mathematics;

namespace GC.Gameplay.Grid
{
    public struct Tile
    {
        public float2 CenterPosition;

        public TileState State;

        public Tile(EditorTile editorTile)
        {
            State = editorTile.State;
            CenterPosition = editorTile.TileCenterPosition;
        }
    }
}
