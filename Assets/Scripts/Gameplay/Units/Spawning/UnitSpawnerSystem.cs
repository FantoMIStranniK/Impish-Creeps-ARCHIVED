using GC.SplineFramework;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct UnitSpawnerSystem : ISystem
{
    private BufferLookup<UnitDeckIndex> unitDeckIndexLookup;
    public void OnCreate(ref SystemState state) 
    {
        //state.World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>()
        unitDeckIndexLookup = state.GetBufferLookup<UnitDeckIndex>(true);
    }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        // get singleton of unit wave

        BeginSimulationEntityCommandBufferSystem.Singleton buggerSystemSingletone;

        if (!SystemAPI.TryGetSingleton(out buggerSystemSingletone))
            return;

        EntityCommandBuffer entityCommandBuffer = buggerSystemSingletone.CreateCommandBuffer(state.World.Unmanaged);

        SplineContainer splineContainer;

        if (!SystemAPI.TryGetSingleton(out splineContainer))
            return;

        UnitDeckComponent unitDeck;

        if (!SystemAPI.TryGetSingleton(out unitDeck))
            return;

        unitDeckIndexLookup.Update(ref state);

        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (UnitSpawnerAspect unitSpawner in SystemAPI.Query<UnitSpawnerAspect>())
        {
            unitSpawner.UpdateSpawnTimer(deltaTime);
            unitSpawner.Spawn(entityCommandBuffer, splineContainer, unitDeck, unitDeckIndexLookup);
        }
    }
}
