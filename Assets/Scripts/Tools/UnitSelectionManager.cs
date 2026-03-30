using System;
using DefaultNamespace;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Tools
{
    public class UnitSelectionManager : MonoBehaviour
    {
        [SerializeField]
        private MouseWorldPosition _mouseWorldPosition;
        
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            SetUnitTargetPosition(_mouseWorldPosition.GetPositon());
        }

        private void SetUnitTargetPosition(Vector3 position)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitMover, Selection>()
                .Build(entityManager);
            
            var unitMovers = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            for (int i = 0; i < unitMovers.Length; i++)
            {
                var mover = unitMovers[i];
                mover.TargetPosition = position;
                unitMovers[i] = mover;
            }
            entityQuery.CopyFromComponentDataArray(unitMovers);
        }
    }
}