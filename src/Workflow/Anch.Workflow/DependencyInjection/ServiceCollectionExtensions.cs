using Anch.DependencyInjection;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Engine;
using Anch.Workflow.StateFactory;
using Anch.Workflow.Storage;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterSyncWorkflowBase(this IServiceCollection services)
    {
        return services
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

    public static IServiceCollection RegisterSyncWorkflowType<TWorkflow>(this IServiceCollection services)
        where TWorkflow : class, IWorkflow
    {
        return services.AddSingleton<TWorkflow>()
                       .AddSingletonFrom<IWorkflow, TWorkflow>();
    }
}