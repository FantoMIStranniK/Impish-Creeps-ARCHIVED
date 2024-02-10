using UnityEngine;
using Unity.Entities;

namespace GC.Gameplay.Units.UnitsQueue
{
    public class UnitQueueAuthoring : MonoBehaviour { }

    public partial class UnitQueueBaker : Baker<UnitQueueAuthoring>
    {
        public override void Bake(UnitQueueAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            var b = AddBuffer<UnitQueueElement>(entity);

            for (int i = 0; i < 10; i++)
            {
                b.Add(new UnitQueueElement { deckIndex = i });
            }
        }
    }
}
