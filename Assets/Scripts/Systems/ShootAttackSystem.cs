using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct ShootAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ShootAttack>();
            state.RequireForUpdate<Target>();
            state.RequireForUpdate<Health>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (shootAttack, target, transform, entity) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Target>, RefRO<LocalTransform>>().WithEntityAccess())
            {
                if (shootAttack.ValueRW.CurrentCooldown > 0)
                {
                    shootAttack.ValueRW.CurrentCooldown -= SystemAPI.Time.DeltaTime;
                    continue;
                }
                
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;
                
                if (!SystemAPI.HasComponent<Unit>(target.ValueRO.TargetEntity))
                    continue;
                
                var targetTransform = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.TargetEntity);
                var distance = math.distance(transform.ValueRO.Position, targetTransform.ValueRO.Position);
                
                if (distance > shootAttack.ValueRO.Range)
                    continue;
                
                Debug.Log($"Entity {entity.Index} shoots at target {target.ValueRO.TargetEntity.Index} with damage {shootAttack.ValueRO.Damage}");
              
                // Наносим урон цели
                if (SystemAPI.HasComponent<Health>(target.ValueRO.TargetEntity))
                {
                    var targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                    targetHealth.ValueRW.CurrentHealth -= shootAttack.ValueRO.Damage;
                    
                    Debug.Log($"Target {target.ValueRO.TargetEntity.Index} health: {targetHealth.ValueRW.CurrentHealth}/{targetHealth.ValueRW.MaxHealth}");
                    
                    // Если здоровье <= 0, можно добавить логику уничтожения цели
                    if (targetHealth.ValueRW.CurrentHealth <= 0)
                    {
                        Debug.Log($"Target {target.ValueRO.TargetEntity.Index} destroyed!");
                        // Здесь можно добавить уничтожение сущности или компонент Dead
                    }
                }
              
                shootAttack.ValueRW.CurrentCooldown = shootAttack.ValueRO.CooldownTime;
            }
        }
    }
}
