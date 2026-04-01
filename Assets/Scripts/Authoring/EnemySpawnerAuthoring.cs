using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        public float SpawnInterval;
        public Transform SpawnTransform;
        
        public class Baker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemySpawner()
                {
                    SpawnInterval = authoring.SpawnInterval,
                    SpawnPosition = authoring.SpawnTransform.position,
                    CurrentCooldown = 0f
                });
            }
        }
    }
    
    public struct EnemySpawner : IComponentData
    {
        public float SpawnInterval;
        public float3 SpawnPosition;
        public float CurrentCooldown;
    }
}
