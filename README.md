# Anch.Framework

Набор .NET-библиотек (`net10.0`) для построения прикладного слоя корпоративных приложений: система прав доступа на основе ролей и контекстов безопасности, ORM-независимый async LINQ, раскрытие иерархий, парсер OData, DI-утилиты и тестовая инфраструктура. Публикуется в NuGet под префиксом `Anch.*`. Лицензия MIT.

> **Подсистема `src/Workflow/` пока не рассматривается: не читать и не анализировать, пока владелец не снимет ограничение.** См. [doc/modules/workflow.md](doc/modules/workflow.md).

## Карта модулей

| Модуль (папка в `src/`) | Назначение | Главные пакеты | Документ |
|---|---|---|---|
| `Core` | Базовые утилиты, Expression-инструменты, `Maybe`, интерфейсы доступа к данным, парсер-комбинаторы, async-примитивы | `Anch.Core`, `Anch.GenericRepository`, `Anch.Parsing`, `Anch.RelativePath`, `Anch.Threading` | [core.md](doc/modules/core.md) |
| `DependencyInjection` | Паттерн `AddXxx(setup)`, прокси-фабрика, валидатор регистраций | `Anch.DependencyInjection` | [dependency-injection.md](doc/modules/dependency-injection.md) |
| `Caching` | Абстракция кэша | `Anch.Caching` | [caching.md](doc/modules/caching.md) |
| `IdentitySource`, `VisualIdentitySource` | Id и отображаемое имя доменного объекта | `Anch.IdentitySource`, `Anch.VisualIdentitySource` | [identity-source.md](doc/modules/identity-source.md) |
| `GenericQueryable` | `GenericToListAsync…` и `WithFetch` без привязки к ORM | `Anch.GenericQueryable`, `…EntityFramework`, `…NHibernate` | [generic-queryable.md](doc/modules/generic-queryable.md) |
| `HierarchicalExpand` | Раскрытие деревьев предок/потомок в SQL-запросах | `Anch.HierarchicalExpand` | [hierarchical-expand.md](doc/modules/hierarchical-expand.md) |
| `OData` | Строка `$filter/$orderby/$top/$skip` → `Expression` | `Anch.OData` | [odata.md](doc/modules/odata.md) |
| `SecuritySystem` | Роли, права, контексты безопасности, RunAs, уведомления, админка (Angular) | `Anch.SecuritySystem`, `…GeneralPermission`, `…VirtualPermission`, `…Notification`, `…Configurator`, `…Testing` | [security-system.md](doc/modules/security-system.md) |
| `Testing` | Пул DI-контейнеров, БД-snapshot'ы, интеграция с xunit v3 | `Anch.Testing`, `Anch.Testing.Xunit`, `Anch.Testing.Database(.Sqlite/.Configuration)` | [testing.md](doc/modules/testing.md) |
| `Workflow` | **Пока не изучать** | — | [workflow.md](doc/modules/workflow.md) |

## Структура репозитория

```
.
├── README.md                  ← вы здесь
├── doc/                       ← документация (начинать с doc/README.md)
├── global.json                # test runner: Microsoft.Testing.Platform
├── .github/workflows/         # build.yml (CI), publish.yml (NuGet)
├── graphify-out/              # граф знаний: в git только graph.json, остальное локально (не исходники)
└── src/
    ├── Anch.Framework.slnx    # единственное решение
    ├── Directory.Build.props  # общие свойства: net10.0, Nullable, LangVersion=latest
    ├── Directory.Packages.props   # центральные версии NuGet
    ├── __RootSolutionItems/   # общие файлы, линкуемые в проекты
    └── <Модуль>/Anch.<Модуль>[.Abstractions|.Domain|.Runtime|…]
```

Типовое устройство модуля (подробно в [doc/architecture.md](doc/architecture.md)):

| Суффикс проекта | Что внутри |
|---|---|
| `.Domain` | Простые модели/enum'ы |
| `.Abstractions` | Публичные интерфейсы |
| `.Runtime` | Реализации |
| *(без суффикса)* | **Точка входа**: `AddXxx(Action<IXxxSetup>)` для `IServiceCollection` |
| `.EntityFramework`, `.NHibernate`, `.Sqlite`, `.Xunit` | Адаптеры под технологию |
| `*.Tests`, `*.IntegrationTests*`, `*.DiTests` | Тесты |

## Быстрый старт

Требуется .NET SDK 10.0.x.

```bash
dotnet restore src/Anch.Framework.slnx
dotnet build src/Anch.Framework.slnx -c Release
dotnet test --project src/Core/Anch.Core.Tests -c Release
```

Полный список тестовых проектов, нюансы сборки Angular-UI конфигуратора и CI: [doc/building-and-testing.md](doc/building-and-testing.md).

Пример подключения (идиома всех модулей):

```csharp
services
    .AddIdentitySource()
    .AddHierarchicalExpand(b => b.AddHierarchicalInfo(v => v.Parent, directedLink, undirectedLink))
    .AddSecuritySystem(b => b
        .SetQueryableSource<MyQueryableSource>()
        .AddSecurityContext<BusinessUnit>(buGuid)
        .AddSecurityRole(MyRoles.Manager, new SecurityRoleInfo(roleGuid))
        .AddDomainSecurity<Order>(DomainSecurityRule.AnyRole /* или роли */, SecurityPath<Order>.Create(o => o.BusinessUnit)));
```

Рабочий полный пример: `src/SecuritySystem/_Example/ExampleApp.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`.

## Памятка для AI-агента

1. Прочитайте [doc/architecture.md](doc/architecture.md), затем [doc/where-to-find.md](doc/where-to-find.md): там указатель «задача → файл».
2. Интерфейс ищите в `*.Abstractions`, реализацию в `*.Runtime`, регистрацию в проекте без суффикса (`DependencyInjection/*Setup.cs`).
3. Образец использования любого API ищите в тестах соответствующего модуля.
4. Код использует C# 14 `extension(...)`-блоки; пути к свойствам везде передаются как `Expression<Func<…>>`.
5. Не редактируйте `graphify-out/`, `bin/`, `obj/`; `src/Workflow/**` не трогать.
