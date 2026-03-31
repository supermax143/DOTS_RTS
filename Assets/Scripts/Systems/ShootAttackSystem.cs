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
                // Уменьшаем кулдаун
                if (shootAttack.ValueRW.CurrentCooldown > 0)
                {
                    shootAttack.ValueRW.CurrentCooldown -= SystemAPI.Time.DeltaTime;
                    continue;
                }
                
                // Проверяем, есть ли цель
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;
                
                // Проверяем, жива ли цель
                if (!SystemAPI.HasComponent<Unit>(target.ValueRO.TargetEntity))
                    continue;
                
                // Получаем позицию цели
                var targetTransform = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.TargetEntity);
                var distance = math.distance(transform.ValueRO.Position, targetTransform.ValueRO.Position);
                
                // Проверяем дистанцию атаки
                if (distance > shootAttack.ValueRO.Range)
                    continue;
                
                // Выполняем выстрел
                Debug.Log($"Entity {entity.Index} shoots at target {target.ValueRO.TargetEntity.Index} with damage {shootAttack.ValueRO.Damage}");
                
                // Здесь можно добавить логику урона
                // Например, добавить компонент Damage к цели или вызвать систему урона
                
                // Устанавливаем кулдаун
                shootAttack.ValueRW.CurrentCooldown = shootAttack.ValueRO.CooldownTime;
            }
        }
    }
}
