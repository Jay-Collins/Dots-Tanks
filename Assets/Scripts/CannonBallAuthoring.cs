using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using Unity.Mathematics;


public class CannonBallAuthoring : MonoBehaviour
{
    private class CannonBallAuthoringBaker : Baker<CannonBallAuthoring>
    {
        public override void Bake(CannonBallAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            
            // By default, components are zero-initialized,
            // so the Velocity field of CannonBall will be float3.zero.
            AddComponent<CannonBall>(entity);
            AddComponent<URPMaterialPropertyBaseColor>(entity);
        }
    }
}

public struct CannonBall : IComponentData
{
    public float3 velocity;
}