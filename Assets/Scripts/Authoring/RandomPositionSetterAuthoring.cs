using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

namespace DefaultNamespace
{
    public class RandomPositionSetterAuthoring : MonoBehaviour
    {
        [SerializeField]
        public float MinDistance;
        
        [SerializeField]
        public float MaxDistance;
        
        public class Baker : Baker<RandomPositionSetterAuthoring>
        {
            public override void Bake(RandomPositionSetterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RandomPositionSetter()
                {
                    MinDistance = authoring.MinDistance,
                    MaxDistance = authoring.MaxDistance,
                });
            }
        }
    }
    
    public struct RandomPositionSetter : IComponentData
    {
        public float MinDistance;
        public float MaxDistance;
    }
}
