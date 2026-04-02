using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class HealthBarAuthoring : MonoBehaviour
    {
        public Transform HealthBarTransform;
        public Transform HealthTarget;
        
        public class Baker : Baker<HealthBarAuthoring>
        {
            public override void Bake(HealthBarAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new HealthBar()
                {
                    HealthBarEntity = GetEntity(authoring.HealthBarTransform, TransformUsageFlags.NonUniformScale),
                    HealthTargetEntity = GetEntity(authoring.HealthTarget, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
    
    public struct HealthBar : IComponentData
    {
        public Entity HealthBarEntity;
        public Entity HealthTargetEntity;
    }
}
