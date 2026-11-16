using Anch.DependencyInjection;
using Anch.Testing.Xunit;
using Anch.Workflow.DependencyInjection;
using Anch.Workflow.Persistence.Memory;

using Microsoft.Extensions.DependencyInjection;

[assembly: AnchTestFramework]

namespace Anch.Workflow.Tests._Base;

public abstract class MultiScopeWorkflowTestBase
{
    private readonly Lazy<IServiceProvider> lazyRootServiceProvider;

    protected MultiScopeWorkflowTestBase()
    {
        this.lazyRootServiceProvider = new Lazy<IServiceProvider>(this.BuildServiceProvider);
    }

    protected IServiceProvider RootServiceProvider => this.lazyRootServiceProvider.Value;

    protected virtual IServiceCollection CreateServices(IServiceCollection services) => services;

    protected virtual void SetupWorkflow(IWorkflowSetup workflowSetup)
    {
        workflowSetup.SetDatabaseProvider<MemoryWorkflowDatabaseProvider>();
    }

    private IServiceProvider BuildServiceProvider()
    {
        return this.CreateServices(new ServiceCollection())
            .AddWorkflow(this.SetupWorkflow)
            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }
}