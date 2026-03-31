using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class FindTargetAuthoring : MonoBehaviour
    {
        [SerializeField]
        public float FindRadius;
        
        [SerializeField]
        public UnitAuthoring.Faction TargetFaction;
        
        [SerializeField]
        public float CooldownTime;
        
        public class Baker : Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FindTarget()
                {
                    FindRadius = authoring.FindRadius,
                    TargetFaction = authoring.TargetFaction,
                    CooldownTime = authoring.CooldownTime,
                    CurrentCooldown = 0f,
                });
            }
        }
    }
    
    public struct FindTarget : IComponentData
    {
        public float FindRadius;
        public UnitAuthoring.Faction TargetFaction;
        public float CooldownTime;
        public float CurrentCooldown;
    }
}
