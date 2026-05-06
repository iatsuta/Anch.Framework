using Anch.DependencyInjection;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Engine;
using Anch.Workflow.Serialization;
using Anch.Workflow.Serialization.Memory;
using Anch.Workflow.StateFactory;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.DependencyInjection;

public class WorkflowSetup : IWorkflowSetup, IServiceInitializer
{
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

            .AddScoped<IInstanceIdGenerator<WorkflowInstance>, RandomIdGenerator<WorkflowInstance>>()
            .AddScoped<IInstanceIdGenerator<StateInstance>, RandomIdGenerator<StateInstance>>()

            .AddKeyedScoped<IWorkflowRepositoryFactory, CachedWorkflowRepositoryFactory>(IWorkflowRepositoryFactory.CacheKey)
            .AddKeyedScoped<IWorkflowRepository, WorkflowRootRepository>(IWorkflowRepository.RootKey)

            .AddSingleton<MemoryWorkflowRootState>()
            .AddScoped<IWorkflowRepositoryFactory, MemoryWorkflowRepositoryFactory>();
    }

    public IWorkflowSetup Add<TWorkflow>()
        where TWorkflow : class, IWorkflow
    {
        this.initActions.Add(services => services.AddSingleton<TWorkflow>()
            .AddSingletonFrom<IWorkflow, TWorkflow>());

        return this;
    }
}