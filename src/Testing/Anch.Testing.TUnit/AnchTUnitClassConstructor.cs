using Microsoft.Extensions.DependencyInjection;

using TUnit.Core.Interfaces;

namespace Anch.Testing.TUnit;

public sealed class AnchTUnitClassConstructor<TEnvironment> :
    IClassConstructor,
    ILastTestInTestSessionEventReceiver
    where TEnvironment : ITestEnvironment, new()
{
    private IServiceProviderPool ServiceProviderPool { get; } = AnchTUnitPool<TEnvironment>.Instance;

    public async Task<object> Create(Type type, ClassConstructorMetadata metadata)
    {
        var ct = TestSessionContext.Current!.SessionCancellationToken;

        var provider = await this.ServiceProviderPool.GetAsync(ct);

        var beforeHookRan = false;

        try
        {
            await provider.RunEnvironmentHooks(EnvironmentHookType.Before, ct);
            beforeHookRan = true;

            var testInstance = ActivatorUtilities.GetServiceOrCreateInstance(provider, type);

            metadata.TestBuilderContext.Events.OnDispose += async (_, _) =>
            {
                try
                {
                    await provider.RunEnvironmentHooks(EnvironmentHookType.After, ct);
                }
                finally
                {
                    await this.ServiceProviderPool.ReleaseAsync(provider, ct);
                }
            };

            return testInstance;
        }
        catch
        {
            if (beforeHookRan)
            {
                await provider.RunEnvironmentHooks(EnvironmentHookType.After, ct);
            }

            await this.ServiceProviderPool.ReleaseAsync(provider, ct);
            throw;
        }
    }

    public ValueTask OnLastTestInTestSession(TestSessionContext context, TestContext testContext) =>
        this.ServiceProviderPool.DisposeAsync();
}