using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

namespace DefaultNamespace
{
    public class AimPositionAuthoring : MonoBehaviour
    {
        public Transform AimTransform;
        
        public class Baker : Baker<AimPositionAuthoring>
        {
            public override void Bake(AimPositionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new AimPosition()
                {
                    AimLocalPosition = authoring.AimTransform.localPosition
                });
            }
        }
    }
    
    public struct AimPosition : IComponentData
    {
        public float3 AimLocalPosition;
    }
}
