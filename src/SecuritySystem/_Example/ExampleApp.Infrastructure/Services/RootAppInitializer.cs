using CommonFramework;
using HierarchicalExpand.Denormalization;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.GeneralPermission.Initialize;

namespace ExampleApp.Infrastructure.Services;

public class RootAppInitializer(IServiceProvider rootServiceProvider) : IInitializer
{
    public const string Key = "RootApp";

    public async Task Initialize(CancellationToken cancellationToken)
    {
        await this.Initialize<IDbSchemaInitializer>(cancellationToken);

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