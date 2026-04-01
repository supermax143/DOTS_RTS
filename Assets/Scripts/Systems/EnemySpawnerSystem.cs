using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct EnemySpawnerSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
            
            foreach (var (spawner, entity) in SystemAPI.Query<RefRW<EnemySpawner>>().WithEntityAccess())
            {
                if (spawner.ValueRW.CurrentCooldown > 0)
                {
                    spawner.ValueRW.CurrentCooldown -= SystemAPI.Time.DeltaTime;
                    continue;
                }
                
                var enemy = state.EntityManager.Instantiate(entitiesReference.Zombie);
                state.EntityManager.SetComponentData(enemy, LocalTransform.FromPosition(spawner.ValueRO.SpawnPosition));
                
                Debug.Log($"Spawned enemy at position {spawner.ValueRO.SpawnPosition}");
                
                spawner.ValueRW.CurrentCooldown = spawner.ValueRO.SpawnInterval;
            }
            
        }
    }
}
