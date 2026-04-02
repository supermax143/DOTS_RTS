---
description: Как добавить новую DOTS систему в проект
---

# Добавление новой DOTS системы

## Шаги для создания новой DOTS системы

### 1. Создание Authoring класса

Создайте файл в `Assets/Scripts/Authoring/` с именем `[SystemName]Authoring.cs`:

```csharp
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class [SystemName]Authoring : MonoBehaviour
    {

        public float SomeParameter;
        public SomeEnum SomeEnumParameter;
        
        public class Baker : Baker<[SystemName]Authoring>
        {
            public override void Bake([SystemName]Authoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new [SystemName]()
                {
                    SomeParameter = authoring.SomeParameter,
                    SomeEnumParameter = authoring.SomeEnumParameter,
                });
            }
        }
    }
    
    public struct [SystemName] : IComponentData
    {
        public float SomeParameter;
        public SomeEnum SomeEnumParameter;
    }
}
```

### 2. Создание DOTS системы

Создайте файл в `Assets/Scripts/Systems/` с именем `[SystemName]System.cs`:

```csharp
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct [SystemName]System : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // Требуемые компоненты для работы системы
            state.RequireForUpdate<[SystemName]>();
            state.RequireForUpdate<LocalTransform>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (component, transform, entity) in 
                SystemAPI.Query<RefRW<[SystemName]>, RefRO<LocalTransform>>()
                .WithEntityAccess())
            {
                // Логика системы здесь
                
                // Пример работы с кулдауном
                if (component.ValueRW.CurrentCooldown > 0)
                {
                    component.ValueRW.CurrentCooldown -= SystemAPI.Time.DeltaTime;
                    continue;
                }
                
                // Основная логика
                // ...
                
                // Установка кулдауна
                component.ValueRW.CurrentCooldown = component.ValueRW.CooldownTime;
            }
        }
    }
}
```

### 3. Добавление компонентов (если нужно)

Если системе нужны дополнительные компоненты, создайте их в Authoring файле:

```csharp
public struct SomeComponent : IComponentData
{
    public Entity TargetEntity;
    public float SomeValue;
}
```

### 4. Работа с физикой (если нужно)

Для работы с Unity Physics:

```csharp
using Unity.Physics;
using Unity.Physics.Systems;

// В OnCreate
state.RequireForUpdate<PhysicsWorldSingleton>();

// В OnUpdate
var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
var collisionWorld = physicsWorld.CollisionWorld;

// Пример сферического поиска
var sphereInput = new PointDistanceInput
{
    Position = transform.ValueRO.Position,
    MaxDistance = component.ValueRO.SearchRadius,
    Filter = new CollisionFilter
    {
        BelongsTo = ~0u,
        CollidesWith = 1u << Layers.Unit, // Используйте Layers.Unit для юнитов
        GroupIndex = 0
    }
};

NativeList<DistanceHit> hitList = new NativeList<DistanceHit>(Allocator.Temp);
collisionWorld.OverlapSphere(sphereInput.Position, sphereInput.MaxDistance, ref hitList, sphereInput.Filter);

for (int i = 0; i < hitList.Length; i++)
{
    var hit = hitList[i];
    // Обработка результатов
}
hitList.Dispose();
```

### 5. Правила именования

- **Authoring классы**: `[SystemName]Authoring`
- **Системы**: `[SystemName]System`
- **Компоненты**: `[SystemName]`
- **Файлы**: Используйте PascalCase для имен файлов

### 6. Структура проекта

```
Assets/Scripts/
├── Authoring/
│   ├── SystemNameAuthoring.cs
│   └── ...
├── Systems/
│   ├── SystemNameSystem.cs
│   └── ...
├── Utils/
│   └── Layers.cs
└── Managers/
    └── ...
```

### 7. Важные моменты

- Используйте `RefRW<T>` для компонентов, которые нужно изменять
- Используйте `RefRO<T>` для компонентов, которые только читаются
- Добавляйте `[BurstCompile]` для оптимизации производительности
- Используйте `SystemAPI.Time.DeltaTime` для работы со временем
- Помните о правильном освобождении `NativeList` с `Allocator.Temp`
- Используйте `Layers.Unit` для фильтрации юнитов в физических запросах

### 7.1. Масштабирование сущностей в DOTS

**Когда нужно масштабировать сущность только по определенным осям:**

Используйте `PostTransformMatrix` вместо изменения `LocalTransform`:

```csharp
// Создаем матрицу масштабирования только по нужным осям
var scaleMatrix = float4x4.Scale(scaleX, 1f, 1f); // Только по X
// или
var scaleMatrix = float4x4.Scale(1f, scaleY, 1f); // Только по Y
// или
var scaleMatrix = float4x4.Scale(scaleX, scaleY, 1f); // По X и Y

// Применяем PostTransformMatrix
SystemAPI.SetComponent(targetEntity, new PostTransformMatrix
{
    Value = scaleMatrix
});
```

**Важные моменты для PostTransformMatrix:**
- Масштабирует только визуальное представление, не затрагивая физику
- Позволяет масштабировать по отдельным осям
- Более производительно чем частые изменения LocalTransform

**TransformUsageFlags для масштабирования:**

При создании entity с неuniform масштабированием используйте:
```csharp
// В Baker классе
HealthBarEntity = GetEntity(authoring.HealthBarTransform, TransformUsageFlags.NonUniformScale),
```

**Когда использовать PostTransformMatrix:**
- Health bars (масштабирование только по ширине)
- UI элементы в DOTS
- Визуальные эффекты с изменением масштаба
- Когда нужно сохранить физические параметры объекта

**Когда использовать LocalTransform:**
- Изменение позиции объекта
- Равномерное масштабирование по всем осям
- Когда масштабирование влияет на физику

### 8. Пример с кулдауном

```csharp
public struct SomeComponent : IComponentData
{
    public float CooldownTime;
    public float CurrentCooldown;
}

// В Authoring
AddComponent(entity, new SomeComponent()
{
    CooldownTime = authoring.CooldownTime,
    CurrentCooldown = 0f,
});

// В системе
if (component.ValueRW.CurrentCooldown > 0)
{
    component.ValueRW.CurrentCooldown -= SystemAPI.Time.DeltaTime;
    continue;
}

// Выполнение действия
component.ValueRW.CurrentCooldown = component.ValueRW.CooldownTime;
```
