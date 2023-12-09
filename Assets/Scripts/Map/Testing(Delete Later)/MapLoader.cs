using UnityEngine;
using UnityEngine.SceneManagement;

namespace GC.Map
{
    public class MapLoader : MonoBehaviour
    {
        public void LoadMap(MapPrefab prefab)
        {
            GlobalSpawnerSystem.MapPrefab = prefab;

            SceneManager.LoadScene("Gameplay_Testing");
        }
    }
}
