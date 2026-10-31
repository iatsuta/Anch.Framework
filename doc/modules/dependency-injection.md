# DependencyInjection

Папка `src/DependencyInjection/`, проект `Anch.DependencyInjection` (+ `Anch.DependencyInjection.Tests`).
Зависит от `Anch.Core` и `Microsoft.Extensions.DependencyInjection.Abstractions`. Это фундамент всех `AddXxx`-методов, см. [architecture.md §3](../architecture.md).

## Что внутри

| Файл | Назначение |
|---|---|
| `IServiceInitializer.cs` | `IServiceInitializer<TContainer>` и `IServiceInitializer` (для `IServiceCollection`): метод `Initialize(services)` |
| `ServiceCollectionExtensions.cs` | `Initialize<TInitializer>(setup)`, `AlreadyInitialized<T>()`, `AddScopedFrom`/`AddSingletonFrom`, `AddServiceProxyFactory`, `BindServiceProxy`, замены регистраций (`Replace*`) |
| `ServiceProviderExtensions.cs` | `GetServiceProxyFactory()` |
| `IServiceCollectionValidator.cs`, `ValidationResult.cs` | Контракт валидатора коллекции сервисов |
| `DuplicateServiceUsageValidator.cs` | Валидатор: находит сервисы, зарегистрированные несколько раз, но запрашиваемые в конструкторе как одиночные зависимости |
| `ServiceCollectionValidationExtensions.cs` | `AddValidator<T>()`, `Validate(options?)` бросает `InvalidOperationException` со списком всех ошибок |
| `ServiceProxy/*` | «Прокси-фабрика»: создание сервиса с подменой типа реализации |

## Service proxy (ServiceProxyFactory)

Задача: создать экземпляр сервиса через DI-совместимый активатор, при этом тип реализации можно **перенаправить** (redirect) настройкой.

- `IServiceProxyFactory.Create<TService>(instanceServiceType, args)`: контракт в `Anch.Core`.
- `ServiceProxyFactory` спрашивает `IServiceProxyTypeRedirector.TryRedirect(type)`, затем создаёт объект через `INativeActivator` (обёртка над `ActivatorUtilities`).
- Настройка: `services.AddServiceProxyFactory(setup => setup.SetRedirect(source, target, replace))` или `BindRedirect(source, binderType, replace)`, где `binderType : IServiceProxyBinder` вычисляет целевой тип.
- Правила цепочки редиректов (`ServiceProxyTypeRedirector`): первый кандидат не должен быть `replace`, каждый следующий обязан быть `replace`. Поддерживаются open generic: target `Foo<>` для source `IFoo<T>`.
- Пример использования: `AncestorDenormalizerServiceProxyBinder` в HierarchicalExpand (`BindServiceProxy(typeof(IAncestorDenormalizer<>), typeof(AncestorDenormalizerServiceProxyBinder<>))`): открытый generic-тип, целевая реализация выбирается по типу доменного объекта.- `IPermissionSystemFactory` в SecuritySystem можно задать через `Func<IServiceProxyFactory, ...>` (`AddPermissionSystem`).

## Валидация сервисов (важно для тестов)

Стандартный контейнер не умеет собирать все ошибки. Типовая сборка в тестах и примерах:

```csharp
services
    .AddXxx(...)
    .AddValidator<DuplicateServiceUsageValidator>()
    .Validate()
    .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
```

Если тест падает с сообщением «The service X has been registered many times…», значит в конструкторе берётся `X`, а зарегистрировано несколько `X`. Нужно брать `IEnumerable<X>` или убрать дубль (`DuplicateServiceUsageValidator(exceptServices)` позволяет задать исключения).

## Тесты

`Anch.DependencyInjection.Tests`: проверки редиректа, валидатора и прокси-фабрики.
