using UnityEngine;
using Unity.Entities;

namespace GC.Gameplay.Status
{
    public class AliveAuthoring : MonoBehaviour
    {
        public int maxHealth;
        public int currentHealth;
    }

    public class AliveBaker : Baker<AliveAuthoring>
    {
        public override void Bake(AliveAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new AliveComponent
            {
                maxHealth = authoring.maxHealth,
                currentHealth = authoring.currentHealth,
            });
        }
    }
}
