using UnityEditor;
using UnityEngine;

namespace GC.Gameplay.Grid
{
#if UNITY_EDITOR
    [CustomEditor(typeof(TileGrid))]
    public class TileGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TileGrid tileGrid = (TileGrid)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(tileGrid.GridTiles)), true);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Grid properties", EditorStyles.boldLabel);

            tileGrid.StartingPoint = EditorGUILayout.Vector2Field("Starting point", tileGrid.StartingPoint);
            tileGrid.GridWidth = EditorGUILayout.IntField("Grid width", tileGrid.GridWidth);
            tileGrid.GridHeight = EditorGUILayout.IntField("Grid height", tileGrid.GridHeight);

            EditorGUILayout.Space();

            tileGrid.HorizontalDirection = (HorizontalDirection)EditorGUILayout.EnumPopup("Horizontal direction", tileGrid.HorizontalDirection);
            tileGrid.VerticalDirection = (VerticalDirection)EditorGUILayout.EnumPopup("Vertical direction", tileGrid.VerticalDirection);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tile properties", EditorStyles.boldLabel);

            tileGrid.TileSize = EditorGUILayout.FloatField("Tile size", tileGrid.TileSize);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Collision processing", EditorStyles.boldLabel);

            tileGrid.SplineOverlapRadiusScale = EditorGUILayout.FloatField("Overlaping radius scale", tileGrid.SplineOverlapRadiusScale);

            tileGrid.SplineCollisionPrecision = EditorGUILayout.IntField("Spline collision precision", tileGrid.SplineCollisionPrecision);

            EditorGUILayout.Space();

            tileGrid.OverlapBoxModifier = EditorGUILayout.FloatField("Overlap box modifier", tileGrid.OverlapBoxModifier);

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(tileGrid.Splines)), true);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            if (GUILayout.Button("Build grid", GUILayout.Width(250f)))
                tileGrid.BuildGrid();

            EditorGUILayout.Space();

            if (GUILayout.Button("Bake collisions", GUILayout.Width(250f)))
                tileGrid.BakeCollision();
        }
    }
#endif
}
