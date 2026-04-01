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
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
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
                
                Debug.Log($"Entity {entity.Index} shoots bullet at target {target.ValueRO.TargetEntity.Index} with damage {shootAttack.ValueRO.Damage}");

                var entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
                
                // Создаем пулю
                var bulletEntity = state.EntityManager.Instantiate(entitiesReference.Bullet);
                
                // Устанавливаем позицию пули
                state.EntityManager.SetComponentData(bulletEntity,
                    LocalTransform.FromPosition(transform.ValueRO.Position));
                
                // Устанавливаем цель для пули
                var targetComponent = SystemAPI.GetComponentRW<Target>(bulletEntity);
                targetComponent.ValueRW.TargetEntity = target.ValueRO.TargetEntity;
                
                // Устанавливаем урон и скорость пули из ShootAttack
                var bulletComponent = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
                bulletComponent.ValueRW.Damage = shootAttack.ValueRO.Damage;
                bulletComponent.ValueRW.Speed = shootAttack.ValueRO.BulletSpeed;
                
                
                
                shootAttack.ValueRW.CurrentCooldown = shootAttack.ValueRO.CooldownTime;
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
