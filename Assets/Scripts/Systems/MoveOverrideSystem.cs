using Systems;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct MoveOverrideSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            foreach (var (moveOverride, unitMover, transform, entity) in SystemAPI.Query<RefRW<MoveOverride>, RefRW<UnitMover>, RefRO<LocalTransform>>().WithEntityAccess())
            {
                // Проверяем включен ли MoveOverride
                if (!SystemAPI.IsComponentEnabled<MoveOverride>(entity))
                    continue;
                
                // Назначаем позицию в UnitMover
                unitMover.ValueRW.TargetPosition = moveOverride.ValueRO.TargetPosition;
                
                // Проверяем достижение позиции
                var distance = math.distance(transform.ValueRO.Position, moveOverride.ValueRO.TargetPosition);
                
                if (UnitMoverSystem.TargetReached(distance))
                {
                    // Позиция достигнута, отключаем MoveOverride
                    state.EntityManager.SetComponentEnabled<MoveOverride>(entity, false);
                    Debug.Log($"Entity {entity.Index} reached target position {moveOverride.ValueRO.TargetPosition}");
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
