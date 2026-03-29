using System;
using DefaultNamespace;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Tools
{
    public class UnitSelectionManager : MonoBehaviour
    {
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            SetUnitTargetPosition(MouseWorldPosition.Instance.GetPositon());
        }

        private void SetUnitTargetPosition(Vector3 position)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitMover>()
                .Build(entityManager);
            
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            var unitMovers = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            for (int i = 0; i < unitMovers.Length; i++)
            {
                var mover = unitMovers[i];
                mover.TargetPosition = position;
                entityManager.SetComponentData(entities[i], mover);
            }
        }
    }
}