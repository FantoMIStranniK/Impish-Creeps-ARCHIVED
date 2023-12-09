using UnityEngine;

namespace GC.Map
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Map/MapPrefab", fileName = "NewMapPrefab", order = 0)]
    public class MapPrefab : ScriptableObject
    {
        [field : SerializeField] public GameObject Splines { get; private set; }
        [field : Space]
        [field : SerializeField] public GameObject Grid { get; private set; }
        [field : Space]
        [field : SerializeField] public GameObject MapDecorations { get; private set; }
    }
}
