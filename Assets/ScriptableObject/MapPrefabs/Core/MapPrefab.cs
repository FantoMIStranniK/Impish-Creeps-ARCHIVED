using UnityEngine;

namespace GC.Map
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Map/MapPrefab", fileName = "NewMapPrefab", order = 0)]
    public class MapPrefab : ScriptableObject
    {
        public MapContainer MapContainer;
    }
}
