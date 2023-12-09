using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[BurstCompile]
public partial struct DisposingSystem : ISystem
{
    [BurstCompile]
    void OnCreate(ref SystemState state) { }
    [BurstCompile]
    void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach ((IsDisposingComponent isDisposing, Entity entity) in SystemAPI.Query<IsDisposingComponent>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity);
        }

        //ecb.Playback(state.EntityManager);
        //ecb.Dispose();
    }//
}
