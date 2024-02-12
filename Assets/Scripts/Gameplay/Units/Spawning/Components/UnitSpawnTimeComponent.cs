using Unity.Entities;

namespace GC.Gameplay.Units.Spawn
{
    public struct UnitSpawnTimeComponent : IComponentData
    {
        public float spawnInterval;
        public float spawnTimer;
        public bool canSpawn;
    }
}
