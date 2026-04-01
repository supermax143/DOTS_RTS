using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(FindTargetSystem))]
    public partial struct ResetTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
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
            
            foreach (var target in SystemAPI.Query<RefRW<Target>>())
            {
                if (target.ValueRW.TargetEntity == Entity.Null)
                {
                    continue;
                }
                
                // Проверяем, существует ли цель
                if (!state.EntityManager.Exists(target.ValueRO.TargetEntity) ||
                    !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.TargetEntity))
                {
                    // Цель не существует, сбрасываем в Null
                    target.ValueRW.TargetEntity = Entity.Null;
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
