using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class ShootAttackAuthoring : MonoBehaviour
    {
        [SerializeField]
        public float CooldownTime;
        
        [SerializeField]
        public float Damage;
        
        [SerializeField]
        public float Range;
        
        [SerializeField]
        public float BulletSpeed;
        
        public class Baker : Baker<ShootAttackAuthoring>
        {
            public override void Bake(ShootAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootAttack()
                {
                    CooldownTime = authoring.CooldownTime,
                    Damage = authoring.Damage,
                    Range = authoring.Range,
                    BulletSpeed = authoring.BulletSpeed,
                    CurrentCooldown = 0f,
                });
            }
        }
    }
    
    public struct ShootAttack : IComponentData
    {
        public float CooldownTime;
        public float Damage;
        public float Range;
        public float BulletSpeed;
        public float CurrentCooldown;
    }
}
