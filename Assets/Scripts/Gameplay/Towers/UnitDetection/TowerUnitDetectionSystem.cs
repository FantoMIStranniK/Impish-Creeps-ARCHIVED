using GC.Units.Movement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TowerUnitDetectionSystem : ISystem 
{
    private ComponentLookup<LocalTransform> localTransformLookup;
    private ComponentLookup<UnitMovementComponent> unitMovementComponentLookup;
    private EntityStorageInfoLookup entityStorageInfo;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        localTransformLookup = state.GetComponentLookup<LocalTransform>(true);
        unitMovementComponentLookup = state.GetComponentLookup<UnitMovementComponent>(true);
        entityStorageInfo = state.GetEntityStorageInfoLookup();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        DetectionCollisionFiltersComponent unitCollisionFilters;

        if (!SystemAPI.TryGetSingleton(out unitCollisionFilters))
            return;
            
        localTransformLookup.Update(ref state);
        unitMovementComponentLookup.Update(ref state);
        entityStorageInfo.Update(ref state);

        EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();
        EntityQuery singletonQuery = state.WorldUnmanaged.EntityManager.CreateEntityQuery(builder);
        CollisionWorld collisionWorld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        singletonQuery.Dispose();

        JobHandle unitDetectionHandle = new UnitDetectionJob
        {
            unitCollisionFilters = unitCollisionFilters,
            unitMovementComponentLookup = unitMovementComponentLookup,
            collisionWorld = collisionWorld,
        }.ScheduleParallel(state.Dependency);

        unitDetectionHandle.Complete();
    }
}

[BurstCompile]
public partial struct UnitDetectionJob : IJobEntity
{
    public DetectionCollisionFiltersComponent unitCollisionFilters;
    [ReadOnly] public CollisionWorld collisionWorld;
    [ReadOnly] public ComponentLookup<UnitMovementComponent> unitMovementComponentLookup;

    [BurstCompile]
    public void Execute(ref TowerRadiusComponent towerRadius, ref LocalTransform localTransform, ref TargetComponent towerTarget)
    {
        NativeList<DistanceHit> unitHits = new NativeList<DistanceHit>(Allocator.Temp);

        collisionWorld.OverlapSphere(localTransform.Position, towerRadius.radius, ref unitHits, unitCollisionFilters.unit);
        
        if (unitHits.Length == 0)
            return;

        //float minDistance = float.MaxValue; /// CLOSE TARGETING
        //int minDistanceIndex = 0;
        //int i = 0;

        //foreach (DistanceHit hit in unitHits)
        //{
        //    if (hit.Fraction < 0.1f)
        //        continue;

        //    if (minDistance > hit.Fraction)
        //    {
        //        minDistance = hit.Fraction;
        //        minDistanceIndex = i;
        //    }

        //    i++;
        //}

        Debug.Log("1");

        float maxTime = float.MinValue; /// FIRST TARGETING
        int maxTimeIndex = 0;
        int i = 0;

        foreach (DistanceHit hit in unitHits)
        {
            if (hit.Fraction < 0.1f)
                continue;

            var movementComponent = unitMovementComponentLookup[hit.Entity];

            if (maxTime < movementComponent.Time)
            {
                maxTime = movementComponent.Time;
                maxTimeIndex = i;
            }

            i++;
        }

        towerTarget.enemy = unitHits[maxTimeIndex].Entity;
    }
}
