using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

namespace GC.Gameplay.Units.Movement
{
    public class UnitDeckAuthoring : MonoBehaviour
    {
        public List<GameObject> deck = new List<GameObject>(10);
    }

    public class UnitDeckBaker : Baker<UnitDeckAuthoring>
    {
        public override void Bake(UnitDeckAuthoring authoring)
        {
            if (authoring.deck.Count != 10)
            {
                Debug.LogError("Deck size has to be exactly 10 elements");
                return;
            }

            Entity entity = GetEntity(TransformUsageFlags.None);
            NativeArray<UnitDeckElement> nativeDeck = new NativeArray<UnitDeckElement>(10, Allocator.Persistent);

            for (int i = 0; i < nativeDeck.Length; i++)
            {
                UnitDeckElement element = nativeDeck[i];

                element.unit = GetEntity(authoring.deck[i].gameObject, TransformUsageFlags.Dynamic);

                nativeDeck[i] = element;
            }

            AddComponent(entity, new UnitDeckComponent());

            AddBuffer<UnitDeckElement>(entity).AddRange(nativeDeck);
        }
    }
}
