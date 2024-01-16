using GC.SplineMovement;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class TowerShooterAuthoring : MonoBehaviour
{
    public GameObject projectile;

    public byte projectileCount;

    public float attackTime;

    public int damage;
    public ushort pierce;

    public float lifeTime;

    public float speed;

    #region Gizmos
#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, speed * lifeTime);
    }

#endif
    #endregion
}

public class TowerShooterBaker : Baker<TowerShooterAuthoring>
{
    public override void Bake(TowerShooterAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new TowerShooterComponent
        {
            projectile = GetEntity(authoring.projectile, TransformUsageFlags.Dynamic),
            projectileCount = authoring.projectileCount,
            attackTime = authoring.attackTime,
            attackTimer = authoring.attackTime,
            damage = authoring.damage,
            pierce = authoring.pierce,
            lifeTime = authoring.lifeTime,
            speed = authoring.speed,
        });
    }
}
