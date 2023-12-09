using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public partial struct EnemyUnitDetectionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) { }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        //foreach ((CollisionFilterComponent detectionFilter, TeamComponent team) in SystemAPI.Query<CollisionFilterComponent, TeamComponent>())
        //{
        //    JobHandle handle = new BuildDetectionBounds
        //    {
        //        detectionFilter = detectionFilter,
        //        team = team,
        //    }.ScheduleParallel(state.Dependency);

        //    handle.Complete();
        //}
    }
}

[BurstCompile]
public partial struct BuildDetectionBounds : IJobEntity
{
    public CollisionFilterComponent detectionFilter;
    public TeamComponent team;

    [BurstCompile]
    public void Execute(Unity.Physics.Aspects.ColliderAspect collider, UnitTag tag, TeamComponent team)
    {

    }
}
