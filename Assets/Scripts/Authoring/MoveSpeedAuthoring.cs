using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class MoveSpeedAuthoring : MonoBehaviour
    {
        public float Speed;
        
        public class Baker : Baker<MoveSpeedAuthoring>
        {
            public override void Bake(MoveSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, 
                    new MoveSpeed
                    {
                        Speed = authoring.Speed
                    } );
            }
        }
        
        
    }
    
    public struct MoveSpeed : IComponentData
    {
        public float Speed;
            
    }
}
