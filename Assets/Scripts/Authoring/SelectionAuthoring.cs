using Unity.Entities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DefaultNamespace
{
    public class SelectionAuthoring : MonoBehaviour
    {

        public GameObject Visual;
        public float VisualScale = 1.5f;
        
        public class Baker : Baker<SelectionAuthoring>
        {
            public override void Bake(SelectionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Selection()
                {
                    Visual = GetEntity(authoring.Visual, TransformUsageFlags.Dynamic),
                    VisualScale = authoring.VisualScale
                });   
                SetComponentEnabled<Selection>(entity, false);
            }
        }
        
    }
}

public struct Selection : IComponentData, IEnableableComponent
{
    public Entity Visual;
    public float VisualScale;
}