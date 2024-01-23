using Unity.Entities;

namespace GC.Gameplay.Status
{
    public struct IsDisposingComponent : IComponentData
    {
        // add system that use this entity to delay the deletion so no bugs happen (for multiplayer)
    }
}
