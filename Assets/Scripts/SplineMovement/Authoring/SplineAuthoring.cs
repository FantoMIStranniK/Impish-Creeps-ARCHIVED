using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
#endif

namespace GC.SplineMovement
{
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
        }
        //
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


#region SplineEditor
#if UNITY_EDITOR

    [EditorTool("Spline Tool", typeof(SplineAuthoring))]
    public class SplineTool : EditorTool, IDrawSelectedHandles
    {
        public void OnDrawHandles()
        {
            SplineAuthoring spline = target as SplineAuthoring;

            for (int i = 0; i < spline.SplineSegments.Count; i++)
            {
                SplineSegment currentSegment = spline.SplineSegments[i];
                SplineSegment previousSegment = i == 0 ? new SplineSegment() : spline.SplineSegments[i - 1];

                (SplineSegment, SplineSegment) segments = MoveStartPoint(currentSegment, previousSegment);

                spline.SplineSegments[i] = segments.Item1;
                if (i > 0)
                    spline.SplineSegments[i - 1] = segments.Item2;

                EditorGUI.BeginChangeCheck();

                //Handles.DrawSolidDisc(currentSegment.EndPoint, new Vector3(0, 1, 0), 1f);

                float3 newPos1 = Handles.PositionHandle(currentSegment.FirstInterpolationPoint, Quaternion.identity);
                float3 newPos2 = Handles.PositionHandle(currentSegment.SecondInterpolationPoint, Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Spline Interpolation Position");

                    currentSegment.FirstInterpolationPoint = newPos1;
                    currentSegment.SecondInterpolationPoint = newPos2;
                    spline.SplineSegments[i] = currentSegment;
                }



                if (i != spline.SplineSegments.Count - 1)
                    continue;

                EditorGUI.BeginChangeCheck();

                Handles.DrawSolidDisc(currentSegment.EndPoint, new Vector3(0, 1, 0), 1f);

                float3 newPos = Handles.PositionHandle(currentSegment.EndPoint, Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Spline End Position");

                    currentSegment.EndPoint = newPos;
                    spline.SplineSegments[i] = currentSegment;
                }


            }
        }

        public (SplineSegment currentSegment, SplineSegment previousSegment) MoveStartPoint(SplineSegment currentSegment, SplineSegment previousSegment)
        {
            EditorGUI.BeginChangeCheck();

            Handles.color = Color.yellow;
            Handles.DrawSolidDisc(currentSegment.StartPoint, new Vector3(0, 1, 0), 1f);

            float3 newPos = Handles.PositionHandle(currentSegment.StartPoint, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Spline Start Position");

                currentSegment.StartPoint = newPos;
                previousSegment.EndPoint = newPos;
            }

            return (currentSegment, previousSegment);
        }

        //public (SplineSegment currentSegment, SplineSegment previousSegment) MoveStartPoint(SplineSegment currentSegment, SplineSegment previousSegment)
        //{
        //    EditorGUI.BeginChangeCheck();

        //    Handles.color = Color.yellow;
        //    Handles.DrawSolidDisc(currentSegment.StartPoint, new Vector3(0, 1, 0), 1f);

        //    float3 newPos = Handles.PositionHandle(currentSegment.StartPoint, Quaternion.identity);

        //    if (EditorGUI.EndChangeCheck())
        //    {
        //        Undo.RecordObject(target, "Spline Start Position");

        //        currentSegment.StartPoint = newPos;
        //        previousSegment.EndPoint = newPos;
        //    }

        //    return (currentSegment, previousSegment);
        //}

        //(i, segment.StartPoint, Quaternion.identity, 10f)
        //public void MoveStartPoint(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        //{


        //}

        [Shortcut("Create New Point", typeof(SceneView), KeyCode.N)]
        static void CreateNewSplinePoint()
        {
            SplineAuthoring[] splines = Selection.GetFiltered<SplineAuthoring>(SelectionMode.TopLevel);

            if (splines.Length == 0)
                return;

            //EditorGUI.BeginChangeCheck();

            for (int i = 0; i < splines.Length; i++)
            {
                if (splines[i].SplineSegments.Count == 0)
                {
                    Debug.Log($"Your spline has no segments! On {splines[i].gameObject} game object");
                    continue;
                }

                int currentSplineSegmentsLength = splines[i].SplineSegments.Count;

                float3 newSplineStartPoint = splines[i].SplineSegments[currentSplineSegmentsLength - 1].EndPoint;

                splines[i].SplineSegments.Add(new SplineSegment
                {
                    StartPoint = newSplineStartPoint,
                    EndPoint = newSplineStartPoint + new float3(5, 0, 0),
                    FirstInterpolationPoint = newSplineStartPoint + new float3(0, 0, 5),
                    SecondInterpolationPoint = newSplineStartPoint + new float3(5, 0, 5),
                });
            }

            //if (EditorGUI.EndChangeCheck())
            //{
            //    int undoIndex = 0;
            //    foreach (SplineAuthoring splineGO in splines)
            //    {
            //        Undo.RecordObject(splineGO, $"Spline Add New Segment {undoIndex}");
            //        undoIndex++;
            //    }
            //}
        }
    }
#endif
#endregion

}
