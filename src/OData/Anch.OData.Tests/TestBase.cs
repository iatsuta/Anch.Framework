using Anch.DependencyInjection;
using Anch.OData.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.OData.Tests;

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