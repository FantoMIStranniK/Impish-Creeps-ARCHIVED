using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct TowerTargetingSystem : ISystem
{
    private ComponentLookup<LocalTransform> localTransformLookup;
    private EntityStorageInfoLookup entityStorageInfo;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        localTransformLookup = state.GetComponentLookup<LocalTransform>(true);
        entityStorageInfo = state.GetEntityStorageInfoLookup();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        localTransformLookup.Update(ref state);
        entityStorageInfo.Update(ref state);


        foreach ((RefRW<TargetComponent> target, TowerRadiusComponent radius, Entity tower) in SystemAPI.Query<RefRW<TargetComponent>, TowerRadiusComponent>().WithEntityAccess())
        {
            if (target.ValueRO.enemy == Entity.Null)
                continue;

            if (!entityStorageInfo.Exists(target.ValueRO.enemy))
            {
                target.ValueRW.enemy = Entity.Null;

                continue;
            }

            if (radius.radius * radius.radius < math.lengthsq(localTransformLookup[tower].Position - localTransformLookup[target.ValueRO.enemy].Position))
            {
                target.ValueRW.enemy = Entity.Null;

                continue;
            }

        }
    }
}
