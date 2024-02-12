using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

namespace GC.Gameplay.Towers
{
#if UNITY_EDITOR
    [EditorTool("Tower Radius Tool", typeof(TowerRadiusAuthoring))]
    public class TowerRadiusEditor : EditorTool, IDrawSelectedHandles
    {
        public void OnDrawHandles()
        {
            TowerRadiusAuthoring radius = target as TowerRadiusAuthoring;

            EditorGUI.BeginChangeCheck();

            Handles.color = new Color(1, 1, 1, 0.5f);
            float newSize = Handles.RadiusHandle(Quaternion.identity, radius.transform.position, radius.radius);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Tower Radius");

                radius.radius = newSize;
            }
        }
    }
#endif
}

