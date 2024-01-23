using Unity.Entities;

namespace GC.Gameplay.Status
{
    public struct DamageComponent : IComponentData
    {
        public int damage;
        public ushort pierce;
        //public DamageType damageType;
    }
}
