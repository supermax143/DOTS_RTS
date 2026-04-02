using Unity.Burst;
using Unity.Entities;
using Unity.Collections;

namespace DefaultNamespace
{
    public partial struct ShootLightRemoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            foreach (var (shootLight, entity) in SystemAPI.Query<RefRW<ShootLight>>().WithEntityAccess())
            {
                // Уменьшаем время жизни
                shootLight.ValueRW.CurrentTime -= SystemAPI.Time.DeltaTime;
                
                // Если время истекло, удаляем entity
                if (shootLight.ValueRW.CurrentTime <= 0f)
                {
                    ecb.DestroyEntity(entity);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
