using GC.SplineFramework;
using System.Security.Principal;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public readonly partial struct UnitSpawnerAspect : IAspect
{
    private readonly Entity entity;

    private readonly RefRW<UnitSpawnTimeComponent> spawnTime;

    [BurstCompile]
    public void UpdateSpawnTimer(float deltaTime)
    {
        spawnTime.ValueRW.spawnTimer += deltaTime;

        if (spawnTime.ValueRO.spawnTimer < spawnTime.ValueRO.spawnInterval)
            return;

        spawnTime.ValueRW.spawnTimer = 0;
        spawnTime.ValueRW.canSpawn = true;
    }
    
    [BurstCompile]
    public void Spawn(EntityCommandBuffer entityCommandBuffer, SplineContainer splineContainer,
        UnitDeckComponent unitDeck, BufferLookup<UnitDeckIndex> unitDeckIndexLookup)
    {
        if (!spawnTime.ValueRW.canSpawn)
            return;

        //Debug.Log($"spawn {unitDeck.deck[0]}");
        //Entity unit = unitDeck.deck[0];
        Entity spawnedEntity = entityCommandBuffer.Instantiate(unitDeck.test);//]);
        entityCommandBuffer.SetComponent(spawnedEntity, new LocalTransform
        {
            Position = splineContainer.GetSplineByIndex(0).SplineSegments[0].StartPoint,
            Rotation = quaternion.identity,
            Scale = 1,
        });

        spawnTime.ValueRW.canSpawn = false;
    }
}
