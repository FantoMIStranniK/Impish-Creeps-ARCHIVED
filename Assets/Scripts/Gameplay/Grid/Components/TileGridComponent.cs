using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using System.Diagnostics;

namespace GC.Gameplay.Grid
{
    [ChunkSerializable]
    public struct TileGridComponent : IComponentData
    {
        public NativeArray<Tile> Tiles;

        public float2 GridOrigin;

        public VerticalDirection VerticalDirection;
        public HorizontalDirection HorizontalDirection;

        public int GridWidth;
        public int GridHeight;

        public float TileSize;

        public Tile GetTile(float3 worldPosition)
        {
            int2 indexes;

            if(!TryGetIndexesByWorldPosition(worldPosition, out indexes))
            {
                UnityEngine.Debug.LogError("ERROR: Tile index is invalid");

                return Tiles[0];
            }

            return GetTile(indexes);
        }

        public Tile GetTile(int2 indexes)
        {
            int index = GetFlatArrayIndex(indexes);

            return Tiles[index];
        }

        public bool TryGetIndexesByWorldPosition(float3 worldPosition, out int2 indexes)
        {
            indexes = int2.zero;

            if(!PositionIsValid(worldPosition))
                return false;

            indexes.x = (int)math.floor((worldPosition.x - GridOrigin.x) / TileSize);
            indexes.y = (int)math.floor((worldPosition.z - GridOrigin.y) / TileSize);

            return true;
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

        public void SetTileState(float3 worldPosition, TileState state)
        {
            int2 indexes;

            if (!TryGetIndexesByWorldPosition(worldPosition, out indexes))
                return;

            SetTileState(indexes, state);
        }

        public TileState GetTileState(int2 indexes)
        {
            int index = GetFlatArrayIndex(indexes);

            return Tiles[index].State;
        }

        public TileState GetTileState(float3 worldPosition)
        {
            int2 indexes;

            if (!TryGetIndexesByWorldPosition(worldPosition, out indexes))
                return TileState.Unavailable;

            return GetTileState(indexes);
        }

        public bool TileIsAvailable(int2 indexes)
        {
            int index = GetFlatArrayIndex(indexes);

            if (index < 0 || index >= Tiles.Length)
                return false;

            return Tiles[index].State == TileState.Vacant;
        }

        public bool TileIsAvailable(float3 worldPosition)
        {
            int2 indexes;

            if (!TryGetIndexesByWorldPosition(worldPosition, out indexes))
                return false;

            return TileIsAvailable(indexes);
        }

        public int GetFlatArrayIndex(int2 indexes)
            => indexes.x + indexes.y * GridWidth;

        private bool PositionIsValid(float3 position)
        {
            float2 twoDimensionalPosition = new float2(position.x * (int)HorizontalDirection, position.z * (int)VerticalDirection);

            if (twoDimensionalPosition.x < GridOrigin.x)
                return false;

            if (twoDimensionalPosition.y < GridOrigin.y)
                return false;

            if (twoDimensionalPosition.x > GridOrigin.x + TileSize * GridWidth)
                return false;

            if (twoDimensionalPosition.y > GridOrigin.y + TileSize * GridHeight)
                return false;

            return true;
        }
    }
}
