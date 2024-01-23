using Unity.Entities;

namespace GC.Utilities
{
    public struct RandomComponent : IComponentData
    {
        public Unity.Mathematics.Random random;
    }
}
