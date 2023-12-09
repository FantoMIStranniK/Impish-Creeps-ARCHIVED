using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public partial struct ProjectileLifeTimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }


    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        JobHandle updateLifeTimeJob = new UpdateLifeTimeJob
        {
            entityCommandBuffer = entityCommandBuffer,
            deltaTime = SystemAPI.Time.DeltaTime,
        }.Schedule(state.Dependency);

        updateLifeTimeJob.Complete();
    }
}

[BurstCompile]
public partial struct UpdateLifeTimeJob : IJobEntity
{
    public EntityCommandBuffer entityCommandBuffer;
    public float deltaTime;

    public void Execute(ref TemporaryComponent projectileLifeTime, ProjectileTag tag, Entity entity)
    {
        projectileLifeTime.lifeTimeRemaning += deltaTime;

        if (projectileLifeTime.lifeTimeRemaning < projectileLifeTime.lifeTime)
            return;

        entityCommandBuffer.DestroyEntity(entity);
    }
}
