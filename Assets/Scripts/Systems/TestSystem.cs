using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct TestSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }
    }
}
