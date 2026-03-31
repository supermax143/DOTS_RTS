using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class HealthAuthoring : MonoBehaviour
    {
        [SerializeField]
        public float MaxHealth;
        
       
        
        public class Baker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Health()
                {
                    MaxHealth = authoring.MaxHealth,
                    CurrentHealth = authoring.MaxHealth,
                });
            }
        }
    }
    
    public struct Health : IComponentData
    {
        public float MaxHealth;
        public float CurrentHealth;
    }
}
