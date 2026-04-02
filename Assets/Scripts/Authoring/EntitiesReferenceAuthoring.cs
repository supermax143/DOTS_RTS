using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class EntitiesReferenceAuthoring : MonoBehaviour
    {
        public GameObject BulletPrefab;
        public GameObject ZombiePrefab;
        public GameObject ShootLightPrefab;
        
        public class Baker : Baker<EntitiesReferenceAuthoring>
        {
            public override void Bake(EntitiesReferenceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                Entity bulletEntity = Entity.Null;
                Entity zombieEntity = Entity.Null;
                Entity shootLightEntity = Entity.Null;
                
                if (authoring.BulletPrefab != null)
                {
                    bulletEntity = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic);
                }
                
                if (authoring.ZombiePrefab != null)
                {
                    zombieEntity = GetEntity(authoring.ZombiePrefab, TransformUsageFlags.Dynamic);
                }
                
                if (authoring.ShootLightPrefab != null)
                {
                    shootLightEntity = GetEntity(authoring.ShootLightPrefab, TransformUsageFlags.Dynamic);
                }
                
                AddComponent(entity, new EntitiesReference()
                {
                    Bullet = bulletEntity,
                    Zombie = zombieEntity,
                    ShootLight = shootLightEntity
                });
            }
        }
    }
    
    public struct EntitiesReference : IComponentData
    {
        public Entity Bullet;
        public Entity Zombie;
        public Entity ShootLight;
    }
}
