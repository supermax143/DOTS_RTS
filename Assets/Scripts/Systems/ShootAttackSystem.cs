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
                
                // // Создаем пулю
                // var bullet = ecb.CreateEntity();
                // ecb.AddComponent(bullet, new Bullet
                // {
                //     Speed = 10f, // Стандартная скорость пули
                //     Damage = shootAttack.ValueRO.Damage
                // });
                //
                // ecb.AddComponent(bullet, new Target
                // {
                //     TargetEntity = target.ValueRO.TargetEntity
                // });
                // ecb.AddComponent(bullet, new LocalTransform
                // {
                //     Position = transform.ValueRO.Position,
                //     Rotation = quaternion.identity,
                //     Scale = 0.2f
                // });
                //
                // shootAttack.ValueRW.CurrentCooldown = shootAttack.ValueRO.CooldownTime;
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
