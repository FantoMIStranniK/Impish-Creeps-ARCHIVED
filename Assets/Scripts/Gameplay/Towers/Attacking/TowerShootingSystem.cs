using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TowerShootingSystem : ISystem
{
    private ComponentLookup<TargetComponent> targetComponentLookup;
    private ComponentLookup<LocalTransform> localTransformLookup;
    private EntityStorageInfoLookup entityStorageInfoLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        targetComponentLookup = state.GetComponentLookup<TargetComponent>();
        localTransformLookup = state.GetComponentLookup<LocalTransform>();
        entityStorageInfoLookup = state.GetEntityStorageInfoLookup();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        targetComponentLookup.Update(ref state);
        localTransformLookup.Update(ref state);
        entityStorageInfoLookup.Update(ref state);

        JobHandle shootingJobHandle = new ShootingJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,

            entityCommandBuffer = entityCommandBuffer,

            targetComponentLookup = targetComponentLookup,
            localTransformLookup = localTransformLookup,
            entityStorageInfoLookup = entityStorageInfoLookup,
        }.Schedule(state.Dependency);

        shootingJobHandle.Complete();
    }
}

[BurstCompile]
public partial struct ShootingJob : IJobEntity
{
    public EntityCommandBuffer entityCommandBuffer;

    public ComponentLookup<TargetComponent> targetComponentLookup;
    [ReadOnly] public ComponentLookup<LocalTransform> localTransformLookup;
    //[ReadOnly] public ComponentLookup<TowerRadiusComponent> towerRadiusComponentLookup;
    [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;

    public float deltaTime;

    [BurstCompile]
    public void Execute(ref TowerShooterComponent shooter, Parent parent, Entity entity)
    {
        shooter.attackTimer += deltaTime;

        if (shooter.attackTimer < shooter.attackTime)
            return;

        RefRO<TargetComponent> towerTarget = targetComponentLookup.GetRefRO(parent.Value);

        if (towerTarget.ValueRO.enemy == Entity.Null || !entityStorageInfoLookup.Exists(towerTarget.ValueRO.enemy))
            return;

        shooter.attackTimer = 0;

        RefRO<LocalTransform> towerLocalTransform = localTransformLookup.GetRefRO(parent.Value);
        RefRO<LocalTransform> towerTargetTransform = localTransformLookup.GetRefRO(towerTarget.ValueRO.enemy);

        Entity spawnedProjectile = entityCommandBuffer.Instantiate(shooter.projectile);
        entityCommandBuffer.SetComponent(spawnedProjectile, new LocalTransform
        {
            Position = towerLocalTransform.ValueRO.Position,
            Rotation = quaternion.LookRotation(towerTargetTransform.ValueRO.Position - towerLocalTransform.ValueRO.Position, new float3(0, 1, 0)),
            Scale = 1,
        });
        entityCommandBuffer.SetComponent(spawnedProjectile, new PhysicsVelocity
        {
            Linear = math.normalize(towerTargetTransform.ValueRO.Position - towerLocalTransform.ValueRO.Position) * shooter.speed,
        });
        entityCommandBuffer.SetComponent(spawnedProjectile, new DamageComponent
        {
            damage = shooter.damage,
            pierce = shooter.pierce,
        });
        entityCommandBuffer.SetComponent(spawnedProjectile, new TemporaryComponent
        {
            lifeTime = shooter.lifeTime,
        });
    }
}
