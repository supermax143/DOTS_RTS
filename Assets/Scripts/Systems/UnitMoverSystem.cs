using DefaultNamespace;
using Systems;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;


namespace Systems
{
    
    public partial struct UnitMoverSystem : ISystem
    {
        private const float STOP_THRESHOLD = 1;
    
        public static bool TargetReached(float distance) 
            => distance < STOP_THRESHOLD;


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new UnitMoverJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            };
            job.ScheduleParallel();

            // foreach (var (transform, mover, velocity) in 
            //          SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>())
            // {
            //     float3 targetPosition = mover.ValueRO.TargetPosition;//MouseWorldPosition.Instance.GetPositon();
            //     var moveDir = math.normalize(targetPosition - transform.ValueRO.Position);
            //
            //     var targetRotation = quaternion.LookRotation(moveDir, math.up());
            //     transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * mover.ValueRO.RotationSpeed);
            //     
            //     velocity.ValueRW.Linear = moveDir * mover.ValueRO.MoveSpeed;
            //     velocity.ValueRW.Angular = float3.zero;
            // }
        }
    }


    [BurstCompile]
    public partial struct UnitMoverJob : IJobEntity
    {
        public float DeltaTime;
        
        public void Execute(ref LocalTransform transform, in UnitMover mover, ref PhysicsVelocity velocity, in Target target)
        {
            var targetPosition = mover.TargetPosition;
            var moveDir = math.normalize(targetPosition - transform.Position);
            var targetRotation = quaternion.LookRotation(moveDir, math.up());
            
            if (UnitMoverSystem.TargetReached(math.distance(transform.Position, targetPosition)))
            {
                if (target.TargetEntity == Entity.Null)
                {
                    transform.Rotation =
                        math.slerp(transform.Rotation, targetRotation, DeltaTime * mover.RotationSpeed);
                }

                velocity.Linear = float3.zero;
                velocity.Angular = float3.zero;
                return;
            }

            if (target.TargetEntity == Entity.Null)
            {
                transform.Rotation = math.slerp(transform.Rotation, targetRotation, DeltaTime * mover.RotationSpeed);
            }
            
            velocity.Linear = moveDir * mover.MoveSpeed;
            velocity.Angular = float3.zero;
        }
    }
}