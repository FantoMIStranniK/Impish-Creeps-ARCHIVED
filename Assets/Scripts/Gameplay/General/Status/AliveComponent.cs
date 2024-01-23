using Unity.Entities;

namespace GC.Gameplay.Status
{
    public struct AliveComponent : IComponentData
    {
        public int maxHealth;
        public int currentHealth;
    }
}
