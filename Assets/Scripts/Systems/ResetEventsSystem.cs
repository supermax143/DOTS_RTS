using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ResetEventsSystem : ISystem
    {
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var selection in SystemAPI.Query<RefRW<Selection>>().WithPresent<Selection>())
            {
                selection.ValueRW.OnSelected = false;
                selection.ValueRW.OnDeselected = false;
            }
        }

    }
}