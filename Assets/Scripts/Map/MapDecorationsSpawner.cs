using Unity.Mathematics;
using UnityEngine;

namespace GC.Map
{
    public class MapDecorationSpawner : MonoBehaviour
    {
        private void Awake()
        {
            GlobalSpawnerSystem.OnMapDecoCreation += CreateMapDeco;
        }

        private void CreateMapDeco()
        {
            var map = GlobalSpawnerSystem.MapPrefab.MapContainer.MapDecorations;

            Instantiate(map, float3.zero, quaternion.identity);

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            GlobalSpawnerSystem.OnMapDecoCreation -= CreateMapDeco;
        }
    }
}
