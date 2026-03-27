using DefaultNamespace;
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
            foreach (var (transform, speed, velocity) in 
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>, RefRW<PhysicsVelocity>>())
            {
                var targetPosition = transform.ValueRO.Position + new float3(speed.ValueRO.Speed, 0, 0);
                var moveDir = math.normalize(targetPosition - transform.ValueRO.Position);
                //transform.ValueRW.Position = targetPosition;
                transform.ValueRW.Rotation = quaternion.LookRotation(moveDir, math.up());
                velocity.ValueRW.Linear = moveDir * speed.ValueRO.Speed;
                velocity.ValueRW.Angular = float3.zero;
            }
        }
    }
}