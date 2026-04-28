using Anch.Core;
using Anch.HierarchicalExpand.Denormalization;
using Anch.SecuritySystem.GeneralPermission.Initialize;

using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.Infrastructure.Services;

public class TestDataInitializer(IServiceProvider rootServiceProvider) : ITestDataInitializer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        await this.Initialize<ISecurityContextInitializer>(cancellationToken);
        await this.Initialize<ISecurityRoleInitializer>(cancellationToken);

        await this.Initialize(ExampleDataInitializer.Key, cancellationToken);

        await this.Initialize<IAncestorDenormalizer>(cancellationToken);
        await this.Initialize<IDeepLevelDenormalizer>(cancellationToken);
    }

    private async Task Initialize<TInitializer>(CancellationToken cancellationToken)
        where TInitializer : class, IInitializer
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        await scope.ServiceProvider.GetRequiredService<TInitializer>().Initialize(cancellationToken);
    }

    private async Task Initialize(object key, CancellationToken cancellationToken)
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        await scope.ServiceProvider.GetRequiredKeyedService<IInitializer>(key).Initialize(cancellationToken);
    }
}