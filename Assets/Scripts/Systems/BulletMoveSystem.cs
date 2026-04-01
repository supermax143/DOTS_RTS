using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct BulletMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Bullet>();
            state.RequireForUpdate<Target>();
            state.RequireForUpdate<LocalTransform>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            foreach (var (bullet, target, transform, entity) in SystemAPI.Query<RefRO<Bullet>, RefRO<Target>, RefRW<LocalTransform>>().WithEntityAccess())
            {
                if (target.ValueRO.TargetEntity == Entity.Null)
                {
                    // Если у пули нет цели, уничтожаем её
                    ecb.DestroyEntity(entity);
                    continue;
                }
                
                // Проверяем, существует ли цель
                if (!state.EntityManager.Exists(target.ValueRO.TargetEntity))
                {
                    ecb.DestroyEntity(entity);
                    continue;
                }
                
                var targetTransform = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.TargetEntity);
                var currentPos = transform.ValueRO.Position;
                var targetPos = targetTransform.ValueRO.Position;
                
                // Проверяем, есть ли у цели компонент AimPosition
                float3 actualTargetPos = targetPos;
                if (SystemAPI.HasComponent<AimPosition>(target.ValueRO.TargetEntity))
                {
                    var aimPosition = SystemAPI.GetComponentRO<AimPosition>(target.ValueRO.TargetEntity);
                    actualTargetPos = targetTransform.ValueRO.TransformPoint(aimPosition.ValueRO.AimLocalPosition);
                }
                
                // Вычисляем направление к цели с учетом AimPosition
                var direction = math.normalize(actualTargetPos - currentPos);
                
                var distanceBefore = math.distance(currentPos, actualTargetPos);
                
                // Перемещаем пулю
                var moveDistance = bullet.ValueRO.Speed * SystemAPI.Time.DeltaTime;
                var newPos = currentPos + direction * moveDistance;
                var distanceAfter = math.distance(newPos, actualTargetPos);
                
                if (distanceAfter >= distanceBefore)
                {
                    newPos = actualTargetPos;
                    distanceAfter = 0;
                }
                
                transform.ValueRW.Position = newPos;
                
                // Проверяем, достигла ли пуля цели

                
                var checkingDistance = 0.0001f;
                if (distanceAfter <= checkingDistance) // Пороговое расстояние для попадания
                {
                    // Наносим урон цели
                    if (SystemAPI.HasComponent<Health>(target.ValueRO.TargetEntity))
                    {
                        var targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                        targetHealth.ValueRW.CurrentHealth -= bullet.ValueRO.Damage;
                        
                        Debug.Log($"Bullet hit target {target.ValueRO.TargetEntity.Index} for {bullet.ValueRO.Damage} damage. Health: {targetHealth.ValueRW.CurrentHealth}/{targetHealth.ValueRW.MaxHealth}");
                    }
                    
                    // Уничтожаем пулю
                    ecb.DestroyEntity(entity);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
