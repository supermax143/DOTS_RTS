using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class EntitiesReferenceAuthoring : MonoBehaviour
    {
        public GameObject BulletPrefab;
        
        public class Baker : Baker<EntitiesReferenceAuthoring>
        {
            public override void Bake(EntitiesReferenceAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                Entity referenceEntity = Entity.Null;
                
                if (authoring.BulletPrefab != null)
                {
                    referenceEntity = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic);
                }
                
                AddComponent(entity, new EntitiesReference()
                {
                    Bullet = referenceEntity
                });
            }
        }
    }
    
    public struct EntitiesReference : IComponentData
    {
        public Entity Bullet;
    }
}
