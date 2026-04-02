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
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            foreach (var (shootAttack, target, transform, unitMover, entity) in 
                     SystemAPI.Query<RefRW<ShootAttack>, RefRO<Target>, RefRW<LocalTransform>,  RefRW<UnitMover>>()
                         .WithDisabled<MoveOverride>()
                         .WithEntityAccess())
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
                
                
                // Если цель не достигнута, двигаемся к ней
                if (distance > shootAttack.ValueRO.Range)
                {
                    unitMover.ValueRW.TargetPosition = targetTransform.ValueRO.Position;
                }
                else
                {
                    // Цель достигнута, стоим на месте
                    unitMover.ValueRW.TargetPosition = transform.ValueRO.Position;
                }
                
                if (distance > shootAttack.ValueRO.Range)
                    continue;

                var moveDir = math.normalize(targetTransform.ValueRO.Position - transform.ValueRO.Position);
                var targetRotation = quaternion.LookRotation(moveDir, math.up());
                transform.ValueRW.Rotation = targetRotation;
                
                Debug.Log($"Entity {entity.Index} shoots bullet at target {target.ValueRO.TargetEntity.Index} with damage {shootAttack.ValueRO.Damage}");

                var entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
                
                // Создаем пулю
                var bulletEntity = state.EntityManager.Instantiate(entitiesReference.Bullet);

                var bulletSpawnPosition = transform.ValueRO.TransformPoint(shootAttack.ValueRO.BulletLocalSpawnPosition);
                
                // Устанавливаем позицию пули
                state.EntityManager.SetComponentData(bulletEntity,
                    LocalTransform.FromPosition(bulletSpawnPosition));
                
                // Устанавливаем цель для пули
                var targetComponent = SystemAPI.GetComponentRW<Target>(bulletEntity);
                targetComponent.ValueRW.TargetEntity = target.ValueRO.TargetEntity;
                
                // Устанавливаем урон и скорость пули из ShootAttack
                var bulletComponent = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
                bulletComponent.ValueRW.Damage = shootAttack.ValueRO.Damage;
                bulletComponent.ValueRW.Speed = shootAttack.ValueRO.BulletSpeed;
               
                shootAttack.ValueRW.OnShoot.Triggered = true;
                shootAttack.ValueRW.OnShoot.ShootPosition = bulletSpawnPosition;
               
                shootAttack.ValueRW.CurrentCooldown = shootAttack.ValueRO.CooldownTime;
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
