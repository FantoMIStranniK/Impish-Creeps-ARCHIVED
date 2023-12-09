using Unity.Entities;

public struct IsDisposingComponent : IComponentData
{
    // add system that use this entity to delay the deletion so no bugs happen (for multiplayer)
}
