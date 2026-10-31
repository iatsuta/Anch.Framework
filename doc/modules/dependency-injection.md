# DependencyInjection

Folder `src/DependencyInjection/`, project `Anch.DependencyInjection` (+ `Anch.DependencyInjection.Tests`).
Depends on `Anch.Core` and `Microsoft.Extensions.DependencyInjection.Abstractions`. This is the foundation of all `AddXxx` methods, see [architecture.md §3](../architecture.md).

## What is inside

| File | Purpose |
|---|---|
| `IServiceInitializer.cs` | `IServiceInitializer<TContainer>` and `IServiceInitializer` (for `IServiceCollection`): the `Initialize(services)` method |
| `ServiceCollectionExtensions.cs` | `Initialize<TInitializer>(setup)`, `AlreadyInitialized<T>()`, `AddScopedFrom`/`AddSingletonFrom`, `AddServiceProxyFactory`, `BindServiceProxy`, registration replacements (`Replace*`) |
| `ServiceProviderExtensions.cs` | `GetServiceProxyFactory()` |
| `IServiceCollectionValidator.cs`, `ValidationResult.cs` | Contract of a service collection validator |
| `DuplicateServiceUsageValidator.cs` | Validator: finds services registered several times but requested in a constructor as single dependencies |
| `ServiceCollectionValidationExtensions.cs` | `AddValidator<T>()`, `Validate(options?)` throws `InvalidOperationException` with the list of all errors |
| `ServiceProxy/*` | The "proxy factory": creates a service with the implementation type substituted |

## Service proxy (ServiceProxyFactory)

Goal: create a service instance through a DI-compatible activator, while the implementation type can be **redirected** by configuration.

- `IServiceProxyFactory.Create<TService>(instanceServiceType, args)`: the contract is in `Anch.Core`.
- `ServiceProxyFactory` asks `IServiceProxyTypeRedirector.TryRedirect(type)`, then creates the object via `INativeActivator` (a wrapper over `ActivatorUtilities`).
- Configuration: `services.AddServiceProxyFactory(setup => setup.SetRedirect(source, target, replace))` or `BindRedirect(source, binderType, replace)`, where `binderType : IServiceProxyBinder` computes the target type.
- Redirect chain rules (`ServiceProxyTypeRedirector`): the first candidate must not be `replace`, every following one must be `replace`. Open generics are supported: target `Foo<>` for source `IFoo<T>`.
- Usage example: `AncestorDenormalizerServiceProxyBinder` in HierarchicalExpand (`BindServiceProxy(typeof(IAncestorDenormalizer<>), typeof(AncestorDenormalizerServiceProxyBinder<>))`): an open generic type whose target implementation is chosen by the domain object type.
- `IPermissionSystemFactory` in SecuritySystem can be set through `Func<IServiceProxyFactory, ...>` (`AddPermissionSystem`).

## Service validation (important for tests)

The standard container cannot collect all errors. Typical setup in tests and examples:

```csharp
services
    .AddXxx(...)
    .AddValidator<DuplicateServiceUsageValidator>()
    .Validate()
    .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
```

If a test fails with "The service X has been registered many times…", a constructor takes `X` while several `X` are registered. Take `IEnumerable<X>` instead or remove the duplicate (`DuplicateServiceUsageValidator(exceptServices)` lets you specify exceptions).

## Tests

`Anch.DependencyInjection.Tests`: tests of the redirect, the validator and the proxy factory.
