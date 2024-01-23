using UnityEngine;

namespace GC.Map
{
    public class MapContainer : MonoBehaviour
    {
        [Tooltip("''Splines'' gameobject is the parent of all spline on this map, the location of the parent is not important.")]
        [field: SerializeField] public GameObject Splines { get; private set; }
        [field: Space]
        [Tooltip("''Grid'' gameobject is the GO which contains component ''TileGrid'', the location of this gameobject is not important.")]
        [field: SerializeField] public GameObject Grid { get; private set; }
        [field: Space]
        [Tooltip("''Decorations'' gameobject is the parent which contains all of the decorations' GOs, the location IS important, and alsways must be (0,0,0)!")]
        [field: SerializeField] public GameObject MapDecorations { get; private set; }
    }
}
