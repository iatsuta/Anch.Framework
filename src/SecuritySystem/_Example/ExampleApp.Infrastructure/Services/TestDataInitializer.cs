using Anch.Core;
using Anch.HierarchicalExpand.Denormalization;
using Anch.SecuritySystem.GeneralPermission.Initialize;

using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.Infrastructure.Services;

public class TestDataInitializer(IServiceProvider rootServiceProvider) : ITestDataInitializer
{
    public async Task Initialize(CancellationToken ct)
    {
        await this.Initialize<ISecurityContextInitializer>(ct);
        await this.Initialize<ISecurityRoleInitializer>(ct);

        await this.Initialize(ExampleDataInitializer.Key, ct);

        await this.Initialize<IAncestorDenormalizer>(ct);
        await this.Initialize<IDeepLevelDenormalizer>(ct);
    }

    private async Task Initialize<TInitializer>(CancellationToken ct)
        where TInitializer : class, IInitializer
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        await scope.ServiceProvider.GetRequiredService<TInitializer>().Initialize(ct);
    }

    private async Task Initialize(object key, CancellationToken ct)
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        await scope.ServiceProvider.GetRequiredKeyedService<IInitializer>(key).Initialize(ct);
    }
}