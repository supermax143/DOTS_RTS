using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct HealthBarSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (healthBar, health) in SystemAPI.Query<RefRW<HealthBar>, RefRO<Health>>())
            {
                if (!state.EntityManager.Exists(healthBar.ValueRO.HealthBarEntity))
                    continue;
                
                // Calculate health percentage
                float healthPercentage = health.ValueRO.CurrentHealth / health.ValueRO.MaxHealth;
                healthPercentage = math.clamp(healthPercentage, 0f, 1f);
                
                // Create scale matrix for X axis only
                var scaleMatrix = float4x4.Scale(healthPercentage, 1f, 1f);
                
                Debug.Log(healthPercentage);
                // Apply PostTransformMatrix
                SystemAPI.SetComponent(healthBar.ValueRO.HealthBarEntity, new PostTransformMatrix
                {
                    Value = scaleMatrix
                });
                
            }
        }
    }
}
