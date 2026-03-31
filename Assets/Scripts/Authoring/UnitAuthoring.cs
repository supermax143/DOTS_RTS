using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class UnitAuthoring : MonoBehaviour
    {
        public enum Faction
        {
            Friendly, Enemy
        }
        
        [SerializeField]
        public Faction UnitFaction;
        
        public class Baker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new Unit()
                {
                    UnitFaction = authoring.UnitFaction,
                });
            }
        }
    }
    
    public struct Unit : IComponentData
    {
        public UnitAuthoring.Faction UnitFaction;
    }
}