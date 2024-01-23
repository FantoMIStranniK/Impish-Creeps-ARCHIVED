using Unity.Entities;

namespace GC.Gameplay.Towers.Projectiles
{
    [InternalBufferCapacity(2)]
    public struct HitList : IBufferElementData
    {
        public Entity entity;
    }
}
