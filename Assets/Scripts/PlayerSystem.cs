using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct PlayerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
            
    }

    // Because OnUpdate accesses a managed object (the camera), we cannot Burst compile 
    // this method, so we don't use the [BurstCompile] attribute here.
    public void OnUpdate(ref SystemState state)
    {
        var movement = new float3(Input.GetAxis("Horizontal"),0 , Input.GetAxis("Vertical"));

        movement *= SystemAPI.Time.DeltaTime;

        foreach (var playerTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Player>())
        {
            // Move the player tank
            playerTransform.ValueRW.Position += movement;
            
            // Move the camera to follow the player
            var cameraTransform = Camera.main.transform;
            cameraTransform.position = playerTransform.ValueRO.Position;
            cameraTransform.position -= 10.0f * (Vector3)playerTransform.ValueRO.Forward(); // move the camera back from the player
            cameraTransform.position += new Vector3(0, 5f, 0); // raise the camera by an offset
            cameraTransform.LookAt(playerTransform.ValueRO.Position); // look at the player
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}