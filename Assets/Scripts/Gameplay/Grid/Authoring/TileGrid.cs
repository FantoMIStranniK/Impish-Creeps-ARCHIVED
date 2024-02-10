using UnityEngine;
using Unity.Mathematics;
using GC.Gameplay.SplineFramework;
using System;

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

    [Serializable]
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
        public EditorTile[] GridTiles = new EditorTile[0];

        #region Baked values

        [SerializeField]
        public int BakedWidth;
        [SerializeField]
        public int BakedHeight;

        [SerializeField]
        public float BakedTileSize;
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
            GridTiles = new EditorTile[BakedWidth * BakedHeight];

            var currentPoint = StartingPoint;

            for (int i = 0; i < BakedHeight; i++)
            {
                for (int j = 0; j < BakedWidth; j++)
                {
                    var centerX = currentPoint.x + BakedTileSize / 2f * (int)HorizontalDirection;
                    var centerY = currentPoint.y + BakedTileSize / 2f * (int)VerticalDirection;

                    EditorTile tile = new EditorTile(TileState.Vacant, new int2(j, i), currentPoint, new float2(centerX, centerY));

                    GridTiles[GetFlatArrayIndex(j, i)] = tile;

                    currentPoint.x += BakedTileSize * (int)HorizontalDirection;
                }

                currentPoint.x = StartingPoint.x;
                currentPoint.y += BakedTileSize * (int)VerticalDirection;
            }
        }

        [ExecuteInEditMode]
        private void SetBakedValues()
        {
            BakedWidth = GridWidth;
            BakedHeight = GridHeight;

            BakedTileSize = TileSize;

            bakedHorizontalDirection = HorizontalDirection;
            bakedVerticalDirection = VerticalDirection;

            bakedTileRadius = TileSize * math.sqrt(2f) * 0.5f;
        }

        #endregion

        #region Collision

        [ExecuteInEditMode]
        public void BakeCollision()
        {
            for (int i = 0; i < BakedHeight; i++)
            {
                for (int j = 0; j < BakedWidth; j++)
                {
                    GridTiles[GetFlatArrayIndex(j, i)].State = TileState.Vacant;

                    BakeCollisionsForSpline(ref GridTiles[GetFlatArrayIndex(j, i)]);

                    BakeCollisionForTerrain(ref GridTiles[GetFlatArrayIndex(j, i)]);
                }
            }
        }

        [ExecuteInEditMode]
        private void BakeCollisionsForSpline(ref EditorTile tile)
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
            => x + y * BakedWidth;

        [ExecuteInEditMode]
        private void BakeCollisionForTerrain(ref EditorTile tile)
        {
            var overlapSize = new Vector3(BakedTileSize, BakedTileSize, BakedTileSize) * 0.5f * OverlapBoxModifier;

            var overlapCenter = new Vector3(tile.TileCenterPosition.x, 0, tile.TileCenterPosition.y);

            var collisions = Physics.OverlapBox(overlapCenter, overlapSize, Quaternion.identity);

            if (collisions.Length != 0)
                tile.State = TileState.Unavailable;
        }

        #endregion

        #region Gizmos
#if UNITY_EDITOR
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

            Vector3 pos = new Vector3(tilePosition.x + (BakedTileSize / 2f) * (int)bakedHorizontalDirection, 0f, tilePosition.y + (BakedTileSize / 2f) * (int)bakedVerticalDirection);

            Vector3 size = new Vector3(BakedTileSize - spacing, 0.25f, BakedTileSize - spacing);

            Gizmos.DrawCube(pos, size);

            Gizmos.color = Color.magenta;

            Gizmos.DrawSphere(new Vector3(tileCenter.x, 0f, tileCenter.y), 0.075f);
        }

        private bool IsLastPointInRow(float2 positionInGrid)
        {
            if (positionInGrid.x == BakedWidth)
                return true;

            if (positionInGrid.y == BakedHeight)
                return true;

            return false;
        }
#endif
    #endregion
    }
}
