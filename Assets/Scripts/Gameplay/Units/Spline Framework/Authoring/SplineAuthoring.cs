using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using GC.Gameplay.SplineFramework.Model;

namespace GC.Gameplay.SplineFramework
{
    [ChunkSerializable]
    public class SplineAuthoring : MonoBehaviour
    {
        [Header("Spline")]
        public List<SplineSegment> SplineSegments = new List<SplineSegment>()
        {
            new SplineSegment
            {
                StartPoint = new float3(0, 0, 0),
                EndPoint = new float3(0, 0, 5),
                FirstInterpolationPoint = new float3(5, 0, 0),
                SecondInterpolationPoint = new float3(5, 0, 5),
            }
        };

        #region Gizmos
#if UNITY_EDITOR
        [Space]
        [Header("Gizmos")]
        [SerializeField] private bool drawBrokenLinePoint = false;
        [SerializeField] private Color brokenLinePointColor = new Color(120f, 0f, 255f, 255f);
        [SerializeField] private float splineLinePointSize = 1f;

        [Space]
        [SerializeField] private Color splineColor = Color.cyan;
        [SerializeField] private Color linearSplineColor = Color.yellow;
        [SerializeField] private Color linearSplineConnectorColor = new Color(255, 255, 255, 0.1f);
        [SerializeField] private float splineLeverSize = 1f;

        [Space]
        [SerializeField] private bool drawSplineSegmentPoints = false;

        private void OnDrawGizmosSelected()
        {
            if (SegmentsAreNull())
                return;

            foreach (var segment in SplineSegments)
            {
                DrawLinearSpline(segment);
            }

            DrawSplineEnds();
        }

        private void OnDrawGizmos()
        {
            if (SegmentsAreNull())
                return;

            foreach (var segment in SplineSegments)
            {
                DrawCubicSpline(segment);
            }

            DrawSplineEnds();
        }

        private void DrawLinearSpline(SplineSegment segment)
        {
            Gizmos.color = linearSplineColor;

            Vector3 cubeSize = new Vector3(splineLeverSize, splineLeverSize, splineLeverSize);

            Gizmos.DrawLine(segment.StartPoint, segment.FirstInterpolationPoint);

            Gizmos.color = linearSplineConnectorColor;
            Gizmos.DrawLine(segment.FirstInterpolationPoint, segment.SecondInterpolationPoint);
            Gizmos.color = linearSplineColor;

            Gizmos.DrawLine(segment.SecondInterpolationPoint, segment.EndPoint);


            Gizmos.DrawCube(segment.FirstInterpolationPoint, cubeSize);
            Gizmos.DrawCube(segment.SecondInterpolationPoint, cubeSize);

            Gizmos.color = splineColor;
            Gizmos.DrawCube(segment.StartPoint, cubeSize);
        }

        private void DrawCubicSpline(SplineSegment segment)
        {
            Vector3 previousPoint = segment.StartPoint;
            Vector3 cubeSize = new Vector3(splineLeverSize, splineLeverSize, splineLeverSize);

            int countOfPoints = SplineSegmentData.COUNT_OF_BROKEN_LINE_POINTS;

            for (int i = 0; i <= countOfPoints; i++)
            {
                Vector3 currentPoint = segment.GetPointOnSegment((float)i / countOfPoints);

                Gizmos.color = splineColor;

                Gizmos.DrawLine(previousPoint, currentPoint);
                if (drawBrokenLinePoint)
                {
                    Gizmos.color = brokenLinePointColor;

                    Gizmos.DrawSphere(currentPoint, splineLinePointSize * splineLinePointSize);

                    previousPoint = currentPoint;
                }
                if (drawSplineSegmentPoints)
                {
                    Gizmos.color = linearSplineColor;

                    Gizmos.DrawCube(segment.StartPoint, cubeSize);
                    Gizmos.DrawCube(segment.EndPoint, cubeSize);
                }

                previousPoint = currentPoint;
            }

        }

        private void DrawSplineEnds()
        {
            Vector3 cubeSize = new Vector3(splineLeverSize, splineLeverSize, splineLeverSize);

            Gizmos.color = Color.green;
            Gizmos.DrawCube(SplineSegments[0].StartPoint, cubeSize);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(SplineSegments[SplineSegments.Count - 1].EndPoint, cubeSize);
        }

        private bool SegmentsAreNull()
            => SplineSegments == null;
#endif
        #endregion
    }

    public class SplineBaker : Baker<SplineAuthoring>
    {
        public override void Bake(SplineAuthoring authoring)
        {
            if (authoring.SplineSegments.Count == 0)
            {
                Debug.LogError($"Your spline {authoring.gameObject} has no segments in it");
                return;
            }

            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            Spline spline = new Spline();

            spline.Init(authoring.SplineSegments.ToNativeArray(Allocator.Persistent));

            AddComponent(entity, spline);
        }
    }
}
