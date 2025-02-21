using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct TankMovementSystem : ISystem
{
    // Like Update, OnUpdate is called every frame as part of Unity ECS.
    // ref means that SystemState is passed by reference.
    // SystemState is a struct provided by ECS, it contains information
    // about the current state of the system.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        // For each entity having a LocalTransform and tank component,
        // we access the LocalTransform and entity ID.
        // Query<LocalTransform>() creates a query to fetch all entities
        // that have the LocalTransform component.
        // .WithAll<Tank>() his method call filters the query to only
        // include entities that also have the Tank component. 
        // .WithEntityAccess() This method allows the query to return
        // not just the components but also the Entity itself for each match.
        // This is useful if you need to reference or modify the entity directly,
        // for example, with the EntityCommandBuffer to add or remove components dynamically.
        
        // This pattern is very efficient for handling large numbers of entities in Unity's ECS because:
        // Batching: It allows for operations to be batched together, potentially benefiting from
        // parallel execution on Unity's job system.
        // Data-Oriented: By querying for components rather than objects, it aligns well with modern
        // CPU architectures, reducing cache misses and improving performance.
        
        //The WithNone<Player> call specifies that the query should exclude any entity that has a Player component,
        //so the player tank will no longer be moved in this loop.
        foreach (var (transform, entity) in
                 SystemAPI.Query<RefRW<LocalTransform>>()
                     .WithAll<Tank>()
                     .WithNone<Player>() // exclude the player tank from the query
                     .WithEntityAccess())
        {
            var pos = transform.ValueRO.Position;

            // This does not modify the actual position of the tank, only the point at
            // which we sample the 3D noise function. This way, every tank is using a
            // different slice and will move along its own different random flow field.
            pos.y = (float)entity.Index;

            // noise.cnoise(pos / 10f) generates coherent noise based on the position
            // divided by 10 to make the noise less frequent
            // var dir = float3.zero; Creates a 3D vector initialized to (0,0,0)
            // This will store our resulting direction
            // math.sincos(angle, out dir.x, out dir.z); math.sincos calculates both sine and cosine
            // of the angle simultaneously (more efficient than calculating them separately)
            // The sine value is stored in dir.x The cosine value is stored in dir.z
            // This effectively creates a 2D direction vector on the XZ plane
            // The * 4.0f * math.PI part determines how many full rotations the angle will go through.
            var angle = (0.5f + noise.cnoise(pos / 10f)) * 4.0f * math.PI;
            var dir = float3.zero;
            math.sincos(angle, out dir.x, out dir.z);

            // Update the LocalTransform.
            transform.ValueRW.Position += dir * dt * 5.0f;
            transform.ValueRW.Rotation = quaternion.RotateY(angle);
        }

        var spin = quaternion.RotateY(SystemAPI.Time.DeltaTime * math.PI);

        foreach (var tank in SystemAPI.Query<RefRW<Tank>>())
        {
            var trans = SystemAPI.GetComponentRW<LocalTransform>(tank.ValueRO.turret);

            // Add a rotation around the Y axis (relative to the parent).
            trans.ValueRW.Rotation = math.mul(spin, trans.ValueRO.Rotation);
        }
    }
}
