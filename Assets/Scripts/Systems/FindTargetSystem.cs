using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Utils;

namespace DefaultNamespace
{
    public partial struct FindTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var collisionWorld = physicsWorld.CollisionWorld;

            NativeList<DistanceHit> hitList = new NativeList<DistanceHit>(Allocator.Temp);
            foreach (var (findTarget, transform, target, entity) in SystemAPI.Query<RefRW<FindTarget>, RefRO<LocalTransform>, RefRW<Target>>().WithEntityAccess())
            {
                // Уменьшаем кулдаун
                if (findTarget.ValueRW.CurrentCooldown > 0)
                {
                    findTarget.ValueRW.CurrentCooldown -= SystemAPI.Time.DeltaTime;
                    continue; // Пропускаем поиск пока кулдаун не закончился
                }
                
                // Сбрасываем кулдаун
                findTarget.ValueRW.CurrentCooldown = findTarget.ValueRO.CooldownTime;
                
                // Создаем сферический поиск
                var sphereInput = new PointDistanceInput
                {
                    Position = transform.ValueRO.Position,
                    MaxDistance = findTarget.ValueRO.FindRadius,
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << Layers.Unit,
                        GroupIndex = 0
                    }
                };
                
                hitList.Clear();
                collisionWorld.OverlapSphere(sphereInput.Position, sphereInput.MaxDistance, ref hitList, sphereInput.Filter);
                
                for (int i = 0; i < hitList.Length; i++)
                {
                    var hit = hitList[i];
                    var hitEntity = hit.Entity;
                    
                    if (hitEntity == entity)
                        continue;
                    
                    if (SystemAPI.HasComponent<Unit>(hitEntity))
                    {
                        var targetUnit = SystemAPI.GetComponentRO<Unit>(hitEntity);
                        
                        if (targetUnit.ValueRO.UnitFaction == findTarget.ValueRO.TargetFaction)
                        {
                            Debug.Log($"Entity {entity.Index} found target {hitEntity.Index} with faction {findTarget.ValueRO.TargetFaction}");
                            target.ValueRW.TargetEntity = hitEntity;
                            
                            break;
                        }
                    }
                }
            }
        }
    }
}
