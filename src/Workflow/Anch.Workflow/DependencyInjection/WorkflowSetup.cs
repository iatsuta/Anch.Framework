using Anch.DependencyInjection;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Engine;
using Anch.Workflow.Persistence;
using Anch.Workflow.StateProcessing;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.DependencyInjection;

public class WorkflowSetup : IWorkflowSetup, IServiceInitializer
{
    private IWorkflowDatabaseProvider? databaseProvider;

    private readonly List<Action<IServiceCollection>> initActions = [];

    public void Initialize(IServiceCollection services)
    {
        services.AddServiceProxyFactory();

        foreach (var initAction in this.initActions)
        {
            initAction(services);
        }

        services
            .AddScoped<ICodeStateProcessorFactory, CodeStateProcessorFactory>()
            .AddScoped<IWorkflowMachineFactory, WorkflowMachineFactory>()
            .AddScoped<IWorkflowHost, WorkflowHost>()

            .AddScopedFrom<IWorkflowExecutor, IWorkflowHost>(host => host.CreateExecutor(WorkflowExecutionPolicy.SingleStep))

            .AddSingleton<IWorkflowSource, WorkflowSource>()

            .AddKeyedScoped<IWorkflowRepositoryFactory, CachedWorkflowRepositoryFactory>(IWorkflowRepositoryFactory.CacheKey)
            .AddKeyedScoped<IWorkflowRepository, WorkflowRootRepository>(IWorkflowRepository.RootKey);

        (this.databaseProvider ?? throw new InvalidOperationException("Database provider is not set")).AddServices(services);
    }

    public IWorkflowSetup SetDatabaseProvider<TWorkflowDatabaseProvider>()
        where TWorkflowDatabaseProvider : IWorkflowDatabaseProvider, new()
    {
        this.databaseProvider = new TWorkflowDatabaseProvider();

        return this;
    }

    public IWorkflowSetup Add<TWorkflow>()
        where TWorkflow : class, IWorkflow
    {
        this.initActions.Add(services => services.AddSingleton<TWorkflow>()
            .AddSingletonFrom<IWorkflow, TWorkflow>());

        return this;
    }
}