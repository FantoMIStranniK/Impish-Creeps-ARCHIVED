using UnityEngine;

public class MapContainer : MonoBehaviour
{
    [field: SerializeField] public GameObject Splines { get; private set; }
    [field: Space]
    [field: SerializeField] public GameObject Grid { get; private set; }
    [field: Space]
    [field: SerializeField] public GameObject MapDecorations { get; private set; }
}
