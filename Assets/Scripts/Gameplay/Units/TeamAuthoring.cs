using Unity.Entities;
using UnityEngine;

public class TeamAuthoring : MonoBehaviour
{
    public byte team;
}

public class TeamBaker : Baker<TeamAuthoring>
{
    public override void Bake(TeamAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new TeamComponent 
        { 
            team = authoring.team,
        });
    }
}
