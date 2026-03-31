using System;
using DefaultNamespace;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Tools
{
    public class UnitSelectionManager : MonoBehaviour
    {
        [SerializeField]
        private MouseWorldPosition _mouseWorldPosition;
        [SerializeField]
        private float _minMultipleSelection = 40;
        [SerializeField]
        private float _unitSpacing = 2;
        [SerializeField]
        private float _circleSpacing = 3;
        
        
        public event Action OnStartSelection;
        public event Action OnEndSelection;

        private Vector2 _selectionStartPosition;

        public static int UnitLayer => LayerMask.NameToLayer("Unit");
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _selectionStartPosition = Input.mousePosition;
                OnStartSelection?.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                var size = GetSelectionRect().size;
                if (size.x + size.y > _minMultipleSelection)
                {
                    UpdateMultipleSelectedUnits();
                }
                else
                {
                    UpdateSingleSelectionUnit();
                }
                    
                OnEndSelection?.Invoke();
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                SetUnitsTargetPosition(_mouseWorldPosition.GetPositon());
            }

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

        private void DeselectAll()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            var selectedUnitsQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Selection>()
                .Build(entityManager);
            
            var selectedUnitsEntities = selectedUnitsQuery.ToEntityArray(Allocator.Temp);
            var selections = selectedUnitsQuery.ToComponentDataArray<Selection>(Allocator.Temp);
            for (int i = 0; i < selectedUnitsEntities.Length; i++)
            {
                entityManager.SetComponentEnabled<Selection>(selectedUnitsEntities[i], false);
                var selection = selections[i];
                selection.OnDeselected = true;
                entityManager.SetComponentData(selectedUnitsEntities[i], selection);
            }
        }
        
        private void UpdateSingleSelectionUnit()
        {
            DeselectAll();
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var query = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            var physicsWorld = query.GetSingleton<PhysicsWorldSingleton>();
            var collisionWorld = physicsWorld.CollisionWorld;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var raycastInput = new RaycastInput()
            {
                Start = ray.GetPoint(0),
                End = ray.GetPoint(9999),
                Filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << UnitLayer
                }
            };

            if (collisionWorld.CastRay(raycastInput, out RaycastHit hit))
            {
                if (entityManager.HasComponent<Unit>(hit.Entity))
                {
                    entityManager.SetComponentEnabled<Selection>(hit.Entity, true);
                    var selection = entityManager.GetComponentData<Selection>(hit.Entity);
                    selection.OnSelected = true;
                    entityManager.SetComponentData(hit.Entity, selection);
                }
            }
        }

        
        private void UpdateMultipleSelectedUnits()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            DeselectAll();
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
                var selection = entityManager.GetComponentData<Selection>(allUnitsEntities[i]);
                selection.OnSelected = true;
                entityManager.SetComponentData(allUnitsEntities[i], selection);
            }
        }

        
        private void SetUnitsTargetPosition(Vector3 position)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitMover, Selection>()
                .Build(entityManager);
            
            var unitMovers = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            var positions = GetRadialPositions(position, unitMovers.Length);
            for (int i = 0; i < unitMovers.Length; i++)
            {
                var mover = unitMovers[i];
                mover.TargetPosition = positions[i];
                unitMovers[i] = mover;
            }
            entityQuery.CopyFromComponentDataArray(unitMovers);
        }
        
        private Vector3[] GetRadialPositions(Vector3 centerPosition, int unitCount)
        {
            if (unitCount <= 0) return Array.Empty<Vector3>();
    
            // Вычисляем количество кругов
            int circleCount = Mathf.CeilToInt((Mathf.Sqrt(8 * unitCount + 1) - 1) / 2);
    
            var positions = new Vector3[unitCount];
            int unitIndex = 0;
    
            for (int circle = 0; circle < circleCount && unitIndex < unitCount; circle++)
            {
                float radius = circle == 0 ? 0 : (circle * _circleSpacing);
        
                int unitsInThisCircle;
                if (circle == 0)
                {
                    // Центральный круг - 1 юнит
                    unitsInThisCircle = Mathf.Min(1, unitCount - unitIndex);
                }
                else
                {
                    // Внешние круги
                    int maxInCircle = Mathf.FloorToInt(2 * Mathf.PI * radius / _unitSpacing);
                    unitsInThisCircle = Mathf.Min(maxInCircle, unitCount - unitIndex);
                }
        
                for (int i = 0; i < unitsInThisCircle && unitIndex < unitCount; i++)
                {
                    Vector3 targetPos;
            
                    if (circle == 0)
                    {
                        // Центральный юнит
                        targetPos = centerPosition;
                    }
                    else
                    {
                        // Юниты на круге
                        float angle = (2f * Mathf.PI * i) / unitsInThisCircle;
                        targetPos = centerPosition + new Vector3(
                            Mathf.Cos(angle) * radius,
                            0,
                            Mathf.Sin(angle) * radius
                        );
                    }
            
                    positions[unitIndex] = targetPos;
                    unitIndex++;
                }
            }
    
            return positions;
        }
        
        
    }
}