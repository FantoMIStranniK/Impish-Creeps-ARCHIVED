using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using Unity.Entities;

namespace GC.Gameplay.Towers
{
    public class TowerRadiusAuthoring : MonoBehaviour
    {
        public float radius;
    }

    public class TowerRadiusBaker : Baker<TowerRadiusAuthoring>
    {
        public override void Bake(TowerRadiusAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TowerRadiusComponent
            {
                radius = authoring.radius,
            });
        }
    }

    #region ShooterEditor
#if UNITY_EDITOR

    [EditorTool("Tower Radius Tool", typeof(TowerRadiusAuthoring))]
    public class TowerRadiusTool : EditorTool, IDrawSelectedHandles
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
    #endregion
}
