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

        private void DeselectAll()
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
                }
            }
        }

        
        private void UpdateMultipleSelectedUnits()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //
            // var selectedUnitsQuery = new EntityQueryBuilder(Allocator.Temp)
            //     .WithAll<Selection>()
            //     .Build(entityManager);
            //
            // var selectedUnitsEntities = selectedUnitsQuery.ToEntityArray(Allocator.Temp);
            // for (int i = 0; i < selectedUnitsEntities.Length; i++)
            // {
            //     entityManager.SetComponentEnabled<Selection>(selectedUnitsEntities[i], false);
            // }
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