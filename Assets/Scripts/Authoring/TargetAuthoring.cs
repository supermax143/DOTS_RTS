using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class TargetAuthoring : MonoBehaviour
    {
        public GameObject TargetEntity;
        
        public class Baker : Baker<TargetAuthoring>
        {
            public override void Bake(TargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                Entity targetEntity = Entity.Null;
                
                if (authoring.TargetEntity != null)
                {
                    targetEntity = GetEntity(authoring.TargetEntity, TransformUsageFlags.Dynamic);
                }
                
                AddComponent(entity, new Target()
                {
                    TargetEntity = targetEntity,
                });
            }
        }
    }
    
    public struct Target : IComponentData
    {
        public Entity TargetEntity;
    }
}
