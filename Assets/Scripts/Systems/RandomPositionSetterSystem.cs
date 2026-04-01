using Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct RandomPositionSetterSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            foreach (var (randomSetter, mover, transform, entity) in 
                SystemAPI.Query<RefRW<RandomPositionSetter>, RefRW<UnitMover>, RefRO<LocalTransform>>()
                .WithEntityAccess())
            {
                var currentPos = transform.ValueRO.Position;
                var targetPos = mover.ValueRO.TargetPosition;
                var distance = math.distance(currentPos, targetPos);
                
                // Если цель достигнута (или не задана), устанавливаем новую случайную позицию
                if (UnitMoverSystem.TargetReached(distance))
                {
                    // Генерируем случайное направление
                    var randomDirection = new float2(
                        UnityEngine.Random.Range(-1f, 1f),
                        UnityEngine.Random.Range(-1f, 1f)
                    );
                    
                    // Нормализуем направление
                    if (math.length(randomDirection) > 0)
                    {
                        randomDirection = math.normalize(randomDirection);
                    }
                    else
                    {
                        randomDirection = new float2(1, 0);
                    }
                    
                    // Генерируем случайную дистанцию в заданном диапазоне
                    var randomDistance = UnityEngine.Random.Range(
                        randomSetter.ValueRO.MinDistance, 
                        randomSetter.ValueRO.MaxDistance
                    );
                    
                    // Вычисляем новую целевую позицию
                    var newTargetPosition = currentPos + new float3(randomDirection.x, 0, randomDirection.y) * randomDistance;
                    
                    // Устанавливаем новую позицию в UnitMover
                    mover.ValueRW.TargetPosition = newTargetPosition;
                    
                    
                    Debug.Log($"Entity {entity.Index} new random target position: {newTargetPosition}");
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

      
    }
}
