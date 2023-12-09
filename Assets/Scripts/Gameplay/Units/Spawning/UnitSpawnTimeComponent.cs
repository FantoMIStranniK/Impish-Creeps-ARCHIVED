using Unity.Entities;

public struct UnitSpawnTimeComponent : IComponentData
{
    public float spawnInterval;
    public float spawnTimer;
    public bool canSpawn;
}
