using UnityEngine;
using UnityEditor;
using Unity.Mathematics;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using GC.Gameplay.SplineFramework.Model;

namespace GC.Gameplay.SplineFramework
{

#if UNITY_EDITOR
    [EditorTool("Spline Tool", typeof(SplineAuthoring))]
    public class SplineEditor : EditorTool, IDrawSelectedHandles
    {
        private bool active;

        public override void OnActivated()
        {
            base.OnActivated();

            active = true;
        }

        public void OnDrawHandles()
        {
            if (!active)
                return;

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

            float3 newPos = Handles.PositionHandle(currentSegment.StartPoint, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Spline Start Position");

                currentSegment.StartPoint = newPos;
                previousSegment.EndPoint = newPos;
            }

            return (currentSegment, previousSegment);
        }

        [Shortcut("Create New Point", typeof(SceneView), KeyCode.N)]
        static void CreateNewSplinePoint()
        {
            SplineAuthoring[] splines = Selection.GetFiltered<SplineAuthoring>(SelectionMode.TopLevel);

            if (splines.Length == 0)
                return;

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
        }

        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();

            active = false;
        }
    }
#endif
}
