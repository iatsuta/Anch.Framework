# Документация Anch.Framework

Индекс. Документация написана так, чтобы человек или AI-агент мог быстро сориентироваться в репозитории.

## Начните здесь

1. [Корневой README](../README.md): что это, карта модулей, быстрый старт.
2. [architecture.md](architecture.md): раскладка репозитория, слои проектов (`Abstractions`/`Runtime`/…), паттерн `AddXxx(Action<IXxxSetup>)`, граф зависимостей, конвенции кода.
3. [where-to-find.md](where-to-find.md): шпаргалка «что где искать» по задачам и по именам типов.
4. [building-and-testing.md](building-and-testing.md): сборка, запуск тестов, CI, публикация, чек-лист нового проекта.

## Модули

| Документ | Модуль | Кратко |
|---|---|---|
| [core.md](modules/core.md) | Core (`Anch.Core`, `GenericRepository`, `Parsing`, `RelativePath`, `Threading`) | Базовые утилиты, `Maybe`, Expression-инструменты, интерфейсы доступа к данным |
| [dependency-injection.md](modules/dependency-injection.md) | `Anch.DependencyInjection` | Паттерн setup, прокси-фабрика сервисов, валидация регистраций |
| [caching.md](modules/caching.md) | Caching | `ICache`/`ICacheProvider` |
| [identity-source.md](modules/identity-source.md) | IdentitySource, VisualIdentitySource | Id и отображаемое имя произвольного доменного типа |
| [generic-queryable.md](modules/generic-queryable.md) | GenericQueryable | ORM-независимый async LINQ и `WithFetch` (EF Core, NHibernate) |
| [hierarchical-expand.md](modules/hierarchical-expand.md) | HierarchicalExpand | Раскрытие иерархий (родители/дети) в запросах |
| [odata.md](modules/odata.md) | OData | Парсер OData-запроса в `Expression` |
| [security-system.md](modules/security-system.md) | SecuritySystem | Роли, права, контексты безопасности, админка |
| [testing.md](modules/testing.md) | Testing | Тестовая инфраструктура: пул DI-контейнеров, БД-snapshot, xunit v3 |
| [workflow.md](modules/workflow.md) | Workflow | **Пока не изучать** |

## Состояние документации

Документ описывает код на момент написания; часть выводов о внутренностях (особенно `SecuritySystem.Runtime/Builders`) сделана по структуре и именам файлов, без построчного чтения; такие места в тексте помечены. При расхождении с кодом правда в коде, а документ нужно поправить.
