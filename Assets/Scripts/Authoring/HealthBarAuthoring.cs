using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class HealthBarAuthoring : MonoBehaviour
    {
        [SerializeField]
        public Transform HealthBarTransform;
       
        
        public class Baker : Baker<HealthBarAuthoring>
        {
            public override void Bake(HealthBarAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new HealthBar()
                {
                    HealthBarEntity = GetEntity(authoring.HealthBarTransform, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
    
    public struct HealthBar : IComponentData
    {
        public Entity HealthBarEntity;
    }
}
