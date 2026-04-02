using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class MeleeAttackAuthoring : MonoBehaviour
    {
        public float Damage = 10f;
        public float Range = 2f;
        public float CooldownTime = 1f;
        
        public class Baker : Baker<MeleeAttackAuthoring>
        {
            public override void Bake(MeleeAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MeleeAttack()
                {
                    Damage = authoring.Damage,
                    Range = authoring.Range,
                    CooldownTime = authoring.CooldownTime,
                    CurrentCooldown = 0f
                });
            }
        }
    }
    
    public struct MeleeAttack : IComponentData
    {
        public float Damage;
        public float Range;
        public float CooldownTime;
        public float CurrentCooldown;
    }
}
