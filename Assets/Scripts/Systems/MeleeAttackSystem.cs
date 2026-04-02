using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct MeleeAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            foreach (var (meleeAttack, target, transform, unitMover, entity) 
                     in SystemAPI.Query<RefRW<MeleeAttack>, RefRO<Target>, RefRW<LocalTransform>, RefRW<UnitMover>>()
                         .WithDisabled<MoveOverride>()
                         .WithEntityAccess())
            {
                // Проверяем есть ли цель
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;
                
                // Проверяем есть ли у цели компонент Unit
                if (!SystemAPI.HasComponent<Unit>(target.ValueRO.TargetEntity))
                    continue;
                
                // Передаем позицию цели в UnitMover
                var targetTransform = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.TargetEntity);
                unitMover.ValueRW.TargetPosition = targetTransform.ValueRO.Position;
                
                var distance = math.distance(transform.ValueRO.Position, targetTransform.ValueRO.Position);
                
                var moveDir = math.normalize(targetTransform.ValueRO.Position - transform.ValueRO.Position);
                var targetRotation = quaternion.LookRotation(moveDir, math.up());
                transform.ValueRW.Rotation = targetRotation;
                
                // Если цель не достигнута, двигаемся к ней
                if (distance > meleeAttack.ValueRO.Range)
                {
                    unitMover.ValueRW.TargetPosition = targetTransform.ValueRO.Position;
                }
                else
                {
                    // Цель достигнута, стоим на месте
                    unitMover.ValueRW.TargetPosition = transform.ValueRO.Position;
                }
                
                // Проверяем кулдаун атаки
                if (meleeAttack.ValueRW.CurrentCooldown > 0)
                {
                    meleeAttack.ValueRW.CurrentCooldown -= SystemAPI.Time.DeltaTime;
                    continue;
                }
                
                // Проверяем расстояние для атаки
                if (distance > meleeAttack.ValueRO.Range)
                    continue;
                
                // Наносим урон цели
                var targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                targetHealth.ValueRW.CurrentHealth -= meleeAttack.ValueRW.Damage;
                
                Debug.Log($"Entity {entity.Index} melee attacks target {target.ValueRO.TargetEntity.Index} with damage {meleeAttack.ValueRW.Damage}");
                
                // Устанавливаем кулдаун
                meleeAttack.ValueRW.CurrentCooldown = meleeAttack.ValueRW.CooldownTime;
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
