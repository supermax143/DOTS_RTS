using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using DefaultNamespace;

namespace DefaultNamespace
{
    public partial struct ShootLightSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
            
            foreach (var (shootAttack, transform, entity) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<LocalTransform>>().WithEntityAccess())
            {
                // Проверяем флаг выстрела
                if (!shootAttack.ValueRO.OnShoot.Triggered)
                {
                    continue;
                }
                // Спавним ShootLight эффект выстрела
                if (entitiesReference.ShootLight != Entity.Null)
                {
                    var shootLightEntity = state.EntityManager.Instantiate(entitiesReference.ShootLight);
                    
                    // Устанавливаем позицию ShootLight
                    state.EntityManager.SetComponentData(shootLightEntity,
                        LocalTransform.FromPosition(shootAttack.ValueRO.OnShoot.ShootPosition));
                }
            }
        }
    }
}
