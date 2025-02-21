using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public GameObject tankPrefab;
    public GameObject cannonBallPrefab;
    public int tankCount;

    class Baker : Baker<ConfigAuthoring>
    { 
        public override void Bake(ConfigAuthoring authoring)
        {
            // The config entity itself doesn’t need transform components,
            // so we use TransformUsageFlags.None
            var entity = GetEntity(authoring, TransformUsageFlags.None);
            AddComponent(entity, new Config
            {   
                // Bake the prefab into entities. GetEntity will return the 
                // root entity of the prefab hierarchy.
                tankPrefab = GetEntity(authoring.tankPrefab, TransformUsageFlags.Dynamic),
                cannonBallPrefab = GetEntity(authoring.cannonBallPrefab, TransformUsageFlags.Dynamic),
                tankCount = authoring.tankCount,
                
            });
        }
    }
}

public struct Config : IComponentData
{
    public Entity tankPrefab;
    public Entity cannonBallPrefab;
    public int tankCount;
}
