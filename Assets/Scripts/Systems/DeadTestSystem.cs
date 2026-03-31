using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct DeadTestSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Health>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            foreach (var (health, entity) in SystemAPI.Query<RefRO<Health>>().WithEntityAccess())
            {
                if (health.ValueRO.CurrentHealth <= 0)
                {
                    Debug.Log($"Entity {entity.Index} is dead, destroying...");
                    ecb.DestroyEntity(entity);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
