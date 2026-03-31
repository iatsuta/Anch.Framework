using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using OData.DependencyInjection;

namespace OData.Tests;

public abstract class TestBase
{
    private IServiceProvider ServiceProvider { get; } = new ServiceCollection()
        .AddOData()
        .AddValidator<DuplicateServiceUsageValidator>()
        .Validate()
        .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

    protected IRawSelectOperationParser RawSelectOperationParser => this.ServiceProvider.GetRequiredService<IRawSelectOperationParser>();

    protected ISelectOperationParser SelectOperationParser => this.ServiceProvider.GetRequiredService<ISelectOperationParser>();
}