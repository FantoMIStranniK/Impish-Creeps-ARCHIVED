using Unity.Entities;

public struct TowerShooterComponent : IComponentData
{
    public Entity projectile;

    public ushort projectileCount;

    public float attackTime;
    public float attackTimer;

    public int damage;
    public ushort pierce;

    public float lifeTime;

    public float speed;
}
