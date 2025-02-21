using Unity.Entities;
using UnityEngine;

public class TankAuthoring : MonoBehaviour
{
    public GameObject turret;
    public GameObject cannon;

    // Baker class responsible for converting GameObject-based tank components into ECS entities.
    // It is a built in part of the entity conversion process, thus the override.
    // Runs during the baking process when converting GameObjects to entities.
    // Baker class is nested to clump the code together for better readability.
    class Baker : Baker<TankAuthoring>
    {
        // Think of Bakers like this:
        // They're the bridge between the traditional Unity GameObject world and the new ECS world.
        // Every MonoBehaviour that needs to be converted to ECS needs a Baker.
        // The Baker defines how to transform your GameObject-based data into ECS components.
        public override void Bake(TankAuthoring authoring)
        {
            // GetEntity returns the Entity baked from the GameObject.
            // We use .Dynamic because we plan to alter the transform.
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, new Tank
            {
                turret = GetEntity(authoring.turret, TransformUsageFlags.Dynamic),
                cannon = GetEntity(authoring.cannon, TransformUsageFlags.Dynamic)
            });
        }
    }
}

// A component that will be added to the root entity of every tank.
// The baking process is specifically converting your Unity GameObjects (which have the TankAuthoring
// MonoBehaviour attached) into ECS Entities that have the Tank struct as a component.
// While the struct is added to the entity of our Tank, we still need the TankAuthoring
// MonoBehavior because that's what is used in the Unity editor. 
public struct Tank : IComponentData
{
    public Entity turret;
    public Entity cannon;
}

