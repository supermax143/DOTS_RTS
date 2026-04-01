using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class BulletAuthoring : MonoBehaviour
    {
        [SerializeField]
        public float Speed;
        
        [SerializeField]
        public float Damage;
        
        public class Baker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Bullet()
                {
                    Speed = authoring.Speed,
                    Damage = authoring.Damage,
                });
            }
        }
    }
    
    public struct Bullet : IComponentData
    {
        public float Speed;
        public float Damage;
    }
}
