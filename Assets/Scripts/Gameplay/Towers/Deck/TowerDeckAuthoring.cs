using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

namespace GC.Gameplay.Towers.Deck
{

    public class TowerDeckAuthoring : MonoBehaviour
    {
        public List<GameObject> deck = new List<GameObject>(6);
    }

    public class TowerDeckBaker : Baker<TowerDeckAuthoring>
    {
        public override void Bake(TowerDeckAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            NativeArray<TowerDeckElement> deckArr = new NativeArray<TowerDeckElement>(6, Allocator.Persistent);

            for (int i = 0; i < authoring.deck.Count; i++)
            {
                TowerDeckElement elm = deckArr[i];
                elm.tower = GetEntity(authoring.deck[i], TransformUsageFlags.Dynamic);
                deckArr[i] = elm;
            }

            AddComponent(entity, new TowerDeck());

            AddBuffer<TowerDeckElement>(entity).AddRange(deckArr);
        }
    }
}
