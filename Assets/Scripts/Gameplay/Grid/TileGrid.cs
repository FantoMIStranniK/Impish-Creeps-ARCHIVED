using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using GC.SplineMovement;

namespace GC.Gameplay.Grid
{
    public enum HorizontalDirection
    {
        Right = 1,
        Left = -1,
    }
    public enum VerticalDirection
    {
        Up = 1,
        Down = -1,
    }

    [ChunkSerializable]
    public class TileGrid : MonoBehaviour
    {
        [Header("Grid properties")]
        [SerializeField]
        public Vector2 StartingPoint = Vector2.zero;
        [SerializeField]
        public int GridWidth = 3;
        [SerializeField]
        public int GridHeight = 3;

        [Space]
        [SerializeField]
        public HorizontalDirection HorizontalDirection = HorizontalDirection.Right;
        [SerializeField]
        public VerticalDirection VerticalDirection = VerticalDirection.Up;

        [Space]
        [Header("Tile properties")]
        [SerializeField]
        public float TileSize = 1f;

        [Space]
        [Header("Collision processing")]
        [SerializeField]
        public float SplineOverlapRadiusScale = 1f;

        [Space]
        [SerializeField]
        public float OverlapBoxModifier = 1f;

        [Space]
        [SerializeField]
        public int SplineCollisionPrecision = 100;

        [Space]
        [SerializeField]
        public SplineAuthoring[] Splines;

        [SerializeField]
        public Tile[] GridTiles = new Tile[0];

        #region Baked values

        [SerializeField]
        private int bakedWidth;
        [SerializeField]
        private int bakedHeight;

        [SerializeField]
        private float bakedTileSize;
        [SerializeField]
        private float bakedTileRadius;

        [SerializeField]
        private HorizontalDirection bakedHorizontalDirection;
        [SerializeField]
        private VerticalDirection bakedVerticalDirection;

        #endregion

        #region Grid building

        [ExecuteInEditMode]
        public void BuildGrid()
        {
            SetBakedValues();

            GenerateGrid();
        }

        [ExecuteInEditMode]
        private void GenerateGrid()
        {
            GridTiles = new Tile[bakedWidth * bakedHeight];

            var currentPoint = StartingPoint;

            for (int i = 0; i < bakedHeight; i++)
            {
                for (int j = 0; j < bakedWidth; j++)
                {
                    var centerX = currentPoint.x + bakedTileSize / 2f * (int)HorizontalDirection;
                    var centerY = currentPoint.y + bakedTileSize / 2f * (int)VerticalDirection;

                    Tile tile = new Tile(TileState.Vacant, new int2(j, i), currentPoint, new float2(centerX, centerY));

                    GridTiles[GetFlatArrayIndex(j, i)] = tile;

                    currentPoint.x += bakedTileSize * (int)HorizontalDirection;
                }

                currentPoint.x = StartingPoint.x;
                currentPoint.y += bakedTileSize * (int)VerticalDirection;
            }
        }

        [ExecuteInEditMode]
        private void SetBakedValues()
        {
            bakedWidth = GridWidth;
            bakedHeight = GridHeight;

            bakedTileSize = TileSize;

            bakedHorizontalDirection = HorizontalDirection;
            bakedVerticalDirection = VerticalDirection;

            bakedTileRadius = TileSize * math.sqrt(2f) * 0.5f;
        }

        #endregion

        #region Collision

        [ExecuteInEditMode]
        public void BakeCollision()
        {
            for (int i = 0; i < bakedHeight; i++)
            {
                for (int j = 0; j < bakedWidth; j++)
                {
                    GridTiles[GetFlatArrayIndex(j, i)].State = TileState.Vacant;

                    BakeCollisionsForSpline(ref GridTiles[GetFlatArrayIndex(j, i)]);

                    BakeCollisionForTerrain(ref GridTiles[GetFlatArrayIndex(j, i)]);
                }
            }
        }

        [ExecuteInEditMode]
        private void BakeCollisionsForSpline(ref Tile tile)
        {
            foreach (var spline in Splines)
            {
                var splineSegments = spline.SplineSegments;

                foreach (var splineSegment in splineSegments)
                {
                    for (int k = 0; k <= SplineCollisionPrecision; k++)
                    {
                        float progress = (float)k / SplineCollisionPrecision;

                        var point = splineSegment.GetPointOnSegment(progress);

                        if (PointIsInTileRange(tile.TileCenterPosition, new float2(point.x, point.z)))
                            tile.State = TileState.Unavailable;
                    }
                }
            }
        }

        [ExecuteInEditMode]
        private bool PointIsInTileRange(float2 tileCenterPosition, float2 point)
        {
            var radius = bakedTileRadius * SplineOverlapRadiusScale;

            var xDifference = point.x - tileCenterPosition.x;
            var yDifference = point.y - tileCenterPosition.y;

            xDifference *= xDifference;
            yDifference *= yDifference;

            var distanceSquared = xDifference + yDifference;

            return distanceSquared < (radius * radius);
        }

        [ExecuteInEditMode]
        private int GetFlatArrayIndex(int x, int y)
            => x + y * bakedWidth;

        [ExecuteInEditMode]
        private void BakeCollisionForTerrain(ref Tile tile)
        {
            var overlapSize = new Vector3(bakedTileSize, bakedTileSize, bakedTileSize) * 0.5f * OverlapBoxModifier;

            var overlapCenter = new Vector3(tile.TileCenterPosition.x, 0, tile.TileCenterPosition.y);

            var collisions = Physics.OverlapBox(overlapCenter, overlapSize, Quaternion.identity);

            if (collisions.Length != 0)
                tile.State = TileState.Unavailable;
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (GridTiles.Length != 0)
                DrawGrid();

            Gizmos.color = Color.green;

            Gizmos.DrawCube(new Vector3(StartingPoint.x, 0f, StartingPoint.y), new Vector3(1f, 1f, 1f));
        }

        private void DrawGrid()
        {
            foreach (var tile in GridTiles)
            {
                var positionInGrid = tile.PositionInGrid;

                if (!IsLastPointInRow(positionInGrid))
                    DrawTile(tile.PositionInWorld, tile.TileCenterPosition, tile.State);

                Gizmos.color = Color.red;

                var positionInWorld = tile.PositionInWorld;

                Gizmos.DrawSphere(new Vector3(positionInWorld.x, 0f, positionInWorld.y), 0.1f);
            }
        }

        private void DrawTile(float2 tilePosition, float2 tileCenter, TileState tileState)
        {
            Gizmos.color = tileState switch
            {
                TileState.Vacant => Color.white,
                TileState.Unavailable => Color.black,
                _ => Color.magenta,
            };

            float spacing = 0.075f;

            Vector3 pos = new Vector3(tilePosition.x + (bakedTileSize / 2f) * (int)bakedHorizontalDirection, 0f, tilePosition.y + (bakedTileSize / 2f) * (int)bakedVerticalDirection);

            Vector3 size = new Vector3(bakedTileSize - spacing, 0.25f, bakedTileSize - spacing);

            Gizmos.DrawCube(pos, size);

            Gizmos.color = Color.magenta;

            Gizmos.DrawSphere(new Vector3(tileCenter.x, 0f, tileCenter.y), 0.075f);
        }

        private bool IsLastPointInRow(float2 positionInGrid)
        {
            if (positionInGrid.x == bakedWidth)
                return true;

            if (positionInGrid.y == bakedHeight)
                return true;

            return false;
        }

        #endregion
    }
}
