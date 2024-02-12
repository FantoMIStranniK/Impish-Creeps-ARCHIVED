using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using GC.Gameplay.Status;

namespace GC.Gameplay.Towers.Projectiles
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    [BurstCompile]
    public partial struct ProjectileDamageSystem : ISystem
    {
        private BufferLookup<HitList> hitListLookup;
        private ComponentLookup<AliveComponent> healthLookup;
        private ComponentLookup<DamageComponent> projectileDamage;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            hitListLookup = state.GetBufferLookup<HitList>();
            healthLookup = state.GetComponentLookup<AliveComponent>(false);
            projectileDamage = state.GetComponentLookup<DamageComponent>(true);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = //new EntityCommandBuffer(Allocator.TempJob);
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

            healthLookup.Update(ref state);//
            hitListLookup.Update(ref state);
            projectileDamage.Update(ref state);
            
            state.Dependency = new ProjectileHitJob()
            {
                enemiesHealth = healthLookup,
                projectileDamage = projectileDamage,
                hitList = hitListLookup,
                deadEntities = new NativeHashSet<Entity>(1, Allocator.TempJob),
                ECB = ecb,
            }.Schedule(simulation, state.Dependency);

            state.Dependency.Complete();
            //ecb.Playback(state.EntityManager);
            //ecb.Dispose();
        }
    }

    [BurstCompile]
    public struct ProjectileHitJob : ITriggerEventsJob
    {
        public ComponentLookup<AliveComponent> enemiesHealth;

        public BufferLookup<HitList> hitList;
        [ReadOnly] public ComponentLookup<DamageComponent> projectileDamage;

        public NativeHashSet<Entity> deadEntities;

        public EntityCommandBuffer ECB;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity projectile = Entity.Null;
            Entity enemy = Entity.Null;

            if (enemiesHealth.HasComponent(triggerEvent.EntityA))
            {
                enemy = triggerEvent.EntityA;
                projectile = triggerEvent.EntityB;
            }

            if (enemiesHealth.HasComponent(triggerEvent.EntityB))
            {
                enemy = triggerEvent.EntityB;
                projectile = triggerEvent.EntityA;
            }

            if (Entity.Null.Equals(projectile) || Entity.Null.Equals(enemy))
                return;

            if (deadEntities.Contains(enemy) || deadEntities.Contains(projectile))
                return;

            DynamicBuffer<HitList> hits = hitList[projectile];

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].entity.Equals(enemy))
                    return;
            }

            hits.Add(new HitList { entity = enemy });

            DamageComponent damage = projectileDamage[projectile];
            RefRW<AliveComponent> hp = enemiesHealth.GetRefRW(enemy);

            hp.ValueRW.currentHealth -= damage.damage;

            if (hp.ValueRO.currentHealth < 1)
            {
                deadEntities.Add(enemy);
                //ECB.DestroyEntity(enemy);
                ECB.AddComponent(enemy, new IsDisposingComponent());
            }

            if (damage.pierce <= hitList[projectile].Length)
            {
                deadEntities.Add(projectile);
                //ECB.DestroyEntity(enemy);
                ECB.AddComponent(projectile, new IsDisposingComponent());
            }
        }
    }
}
