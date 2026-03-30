using System;
using DefaultNamespace;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Tools
{
    public class UnitSelectionManager : MonoBehaviour
    {
        [SerializeField]
        private MouseWorldPosition _mouseWorldPosition;

        public event Action OnStartSelection;
        public event Action OnEndSelection;

        private Vector2 _selectionStartPosition;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _selectionStartPosition = Input.mousePosition;
                OnStartSelection?.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                UpdateSelectedUnits();
                OnEndSelection?.Invoke();
            }
            
            if (!Input.GetMouseButtonDown(1))
            {
                return;
            }

            SetUnitTargetPosition(_mouseWorldPosition.GetPositon());
        }


        public Rect GetSelectionRect()
        {
            var lowerLeftCorner = new Vector2()
            {
                x = Mathf.Min(_selectionStartPosition.x, Input.mousePosition.x),
                y = Mathf.Min(_selectionStartPosition.y, Input.mousePosition.y)
            };
            var upperRightCorner = new Vector2()
            {
                x = Mathf.Max(_selectionStartPosition.x, Input.mousePosition.x),
                y = Mathf.Max(_selectionStartPosition.y, Input.mousePosition.y)
            };
            return new Rect()
            {
                min = lowerLeftCorner,
                max = upperRightCorner
            };
        }
        private void UpdateSelectedUnits()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            var selectedUnitsQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Selection>()
                .Build(entityManager);
            
            var selectedUnitsEntities = selectedUnitsQuery.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < selectedUnitsEntities.Length; i++)
            {
                entityManager.SetComponentEnabled<Selection>(selectedUnitsEntities[i], false);
            }
            
            var allUnitsQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform, Unit>()
                .Build(entityManager);

            var selectionRect = GetSelectionRect();
            var allTransforms = allUnitsQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var allUnitsEntities = allUnitsQuery.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < allTransforms.Length; i++)
            {
                var unitTransform = allTransforms[i];
                if (!selectionRect.Contains(Camera.main.WorldToScreenPoint(unitTransform.Position)))
                {
                    continue;
                }
                entityManager.SetComponentEnabled<Selection>(allUnitsEntities[i], true);
            }
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