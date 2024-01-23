using Unity.Entities;

namespace GC.Gameplay
{
    public struct TargetComponent : IComponentData
    {
        public Entity enemy;
    }
}
