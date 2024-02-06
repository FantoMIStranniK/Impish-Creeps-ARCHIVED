using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class UnitQueueAuthoring : MonoBehaviour
{
}

public partial class UnitQueueBaker : Baker<UnitQueueAuthoring>
{
    public override void Bake(UnitQueueAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        var b =  AddBuffer<UnitQueueElement>(entity);

        b.Add(new UnitQueueElement { deckIndex = 0 });
        b.Add(new UnitQueueElement { deckIndex = 1 });
        b.Add(new UnitQueueElement { deckIndex = 2 });
        b.Add(new UnitQueueElement { deckIndex = 3 });
        b.Add(new UnitQueueElement { deckIndex = 4 });
        b.Add(new UnitQueueElement { deckIndex = 5 });
        b.Add(new UnitQueueElement { deckIndex = 6 });
        b.Add(new UnitQueueElement { deckIndex = 7 });
        b.Add(new UnitQueueElement { deckIndex = 8 });
        b.Add(new UnitQueueElement { deckIndex = 9 });
    }
}
