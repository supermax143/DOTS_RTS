using DefaultNamespace;
using Tools;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine.PlayerLoop;

namespace Systems
{
    public partial struct UnitMoverSystem : ISystem
    {
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, mover, velocity) in 
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>())
            {
                float3 targetPosition = mover.ValueRO.TargetPosition;//MouseWorldPosition.Instance.GetPositon();
                var moveDir = math.normalize(targetPosition - transform.ValueRO.Position);

                var targetRotation = quaternion.LookRotation(moveDir, math.up());
                transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * mover.ValueRO.RotationSpeed);
                
                velocity.ValueRW.Linear = moveDir * mover.ValueRO.MoveSpeed;
                velocity.ValueRW.Angular = float3.zero;
            }
        }
    }
}