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
        foreach (var initAction in this.initActions)
        {
            initAction(services);
        }

        services
            .AddSingleton<IStateFactoryCache, StateFactoryCache>()
            .AddScoped<IWorkflowMachineFactory, WorkflowMachineFactory>()
            .AddScoped<IWorkflowHost, WorkflowHost>()

            .AddScoped<ICodeStateResolver, CodeStateResolver>()

            .AddSingleton<IWorkflowSource, WorkflowSource>()
            .AddScoped<IInstanceIdGenerator<WorkflowInstance>, RandomIdGenerator<WorkflowInstance>>()
            .AddScoped<IInstanceIdGenerator<StateInstance>, RandomIdGenerator<StateInstance>>()
            .AddScoped<IWorkflowStorage, WorkflowStorage>()
            .AddScoped<ISpecificWorkflowStorageSource, MemCachedSpecificWorkflowStorageSource>()
            .AddScoped<ISpecificWorkflowExternalStorageSource, MemorySpecificWorkflowExternalStorageSource>();
    }

    public IWorkflowSetup Add<TWorkflow>()
        where TWorkflow : class, IWorkflow
    {
        this.initActions.Add(services => services.AddSingleton<TWorkflow>()
            .AddSingletonFrom<IWorkflow, TWorkflow>());

        return this;
    }
}