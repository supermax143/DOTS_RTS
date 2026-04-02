using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class ShootLightAuthoring : MonoBehaviour
    {
        [SerializeField]
        public float LifeTime = 0.5f;
        
        public class Baker : Baker<ShootLightAuthoring>
        {
            public override void Bake(ShootLightAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootLight()
                {
                    LifeTime = authoring.LifeTime,
                    CurrentTime = authoring.LifeTime
                });
            }
        }
    }
    
    public struct ShootLight : IComponentData
    {
        public float LifeTime;
        public float CurrentTime;
    }
}
