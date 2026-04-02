using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class MoveOverrideAuthoring : MonoBehaviour
    {
        public float3 TargetPosition;
        
        public class Baker : Baker<MoveOverrideAuthoring>
        {
            public override void Bake(MoveOverrideAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MoveOverride()
                {
                    TargetPosition = authoring.TargetPosition,
                });
                
                SetComponentEnabled<MoveOverride>(entity, false);
            }
        }
    }
    
    public struct MoveOverride : IComponentData, IEnableableComponent
    {
        public float3 TargetPosition;
    }
}
