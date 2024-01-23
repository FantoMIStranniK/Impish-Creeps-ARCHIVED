using Unity.Entities;

namespace GC.Gameplay.Status
{
    public struct TemporaryComponent : IComponentData
    {
        public float lifeTime;
        public float lifeTimeRemaning;
    }
}
