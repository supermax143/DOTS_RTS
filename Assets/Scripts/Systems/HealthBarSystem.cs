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
            var cameraForward = Vector3.zero;
            if (Camera.main != null)
            {
                cameraForward = Camera.main.transform.forward;
            }
            
            foreach (var (healthBar, barTransform) in SystemAPI.Query<RefRW<HealthBar>, RefRW<LocalTransform>>())
            {
                if (!state.EntityManager.Exists(healthBar.ValueRO.HealthBarEntity) || 
                    !state.EntityManager.Exists(healthBar.ValueRO.HealthTargetEntity))
                    continue;

                var parentTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.HealthTargetEntity);
                
                barTransform.ValueRW.Rotation = 
                    parentTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
                
                    
                // Get health from target entity
                var health = SystemAPI.GetComponentRO<Health>(healthBar.ValueRO.HealthTargetEntity);
                
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
