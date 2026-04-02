using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class ShootAttackAuthoring : MonoBehaviour
    {
        public float CooldownTime;
        public float Damage;
        public float Range;
        public float BulletSpeed;
        public Transform ShootTransform;
        
        
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
                    BulletLocalSpawnPosition = authoring.ShootTransform.localPosition
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
        public float3 BulletLocalSpawnPosition;
        public ShootEvent OnShoot;
        public struct ShootEvent
        {
            public bool Triggered;
            public float3 ShootPosition;
        };
    }
}
