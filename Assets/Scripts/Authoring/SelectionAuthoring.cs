using Unity.Entities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DefaultNamespace
{
    public class SelectionAuthoring : MonoBehaviour
    {
        
        public class Baker : Baker<SelectionAuthoring>
        {
            public override void Bake(SelectionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Selection()
                {
                    
                });   
                SetComponentEnabled<Selection>(entity, false);
            }
        }
        
    }
}

public struct Selection : IComponentData, IEnableableComponent
{
    
}