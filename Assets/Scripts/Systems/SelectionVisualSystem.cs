using DefaultNamespace;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial struct SelectionVisualSystem : ISystem
    {


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var selection in SystemAPI.Query<RefRW<Selection>>().WithPresent<Selection>())
            {
                if (selection.ValueRO.OnSelected)
                {
                    var transform = SystemAPI.GetComponentRW<LocalTransform>(selection.ValueRO.Visual);
                    transform.ValueRW.Scale = selection.ValueRO.VisualScale;
                }
                
                if (selection.ValueRO.OnDeselected)
                {
                    var transform = SystemAPI.GetComponentRW<LocalTransform>(selection.ValueRO.Visual);
                    transform.ValueRW.Scale = 0;
                }
            }
            
        }
    }
}