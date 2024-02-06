using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace GC.Gameplay.Grid
{
    public struct TileGridComponent : IComponentData
    {
        public NativeArray<Tile> Tiles;

        public float2 GridOrigin;

        public int GridWidth;
        public int GridHeight;

        public float TileSize;

        public Tile GetTile(float3 worldPosition)
        {
            int2 indexes = GetIndexesByWorldPosition(worldPosition);

            int index = GetFlatArrayIndex(indexes);

            return Tiles[index];
        }

        public int2 GetIndexesByWorldPosition(float3 worldPosition)
        {
            int2 indexes = new int2(0, 0);

            indexes.x = (int)math.floor((worldPosition.x - GridOrigin.x) / TileSize);
            indexes.y = (int)math.floor((worldPosition.y - GridOrigin.y) / TileSize);

            return indexes;
        }

        public void SetTileState(int2 indexes, TileState state)
        {
            if (!(indexes.x >= 0 && indexes.y >= 0 && indexes.x < GridWidth && indexes.y < GridHeight))
                return;

            int index = GetFlatArrayIndex(indexes);

            var tile = Tiles[index];

            tile.State = state;

            Tiles[index] = tile;
        }

        public void SetTileState(float3 worldPos, TileState state)
        {
            int2 indexes = GetIndexesByWorldPosition(worldPos);

            SetTileState(indexes, state);
        }

        public TileState GetTileState(int2 indexes)
        {
            int index = GetFlatArrayIndex(indexes);

            return Tiles[index].State;
        }

        public TileState GetTileState(float3 worldPosition)
        {
            int2 indexes = GetIndexesByWorldPosition(worldPosition);

            return GetTileState(indexes);
        }

        public bool TileIsAvailable(int2 indexes)
        {
            int index = GetFlatArrayIndex(indexes);

            return Tiles[index].State == TileState.Vacant;
        }

        public bool TileIsAvailable(float3 worldPosition)
        {
            int2 indexes = GetIndexesByWorldPosition(worldPosition);

            return TileIsAvailable(indexes);
        }

        public int GetFlatArrayIndex(int2 indexes)
            => indexes.x + indexes.y * GridWidth;
    }
}
