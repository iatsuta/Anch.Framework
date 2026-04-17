using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.Engine;
using SyncWorkflow.StateFactory;
using SyncWorkflow.Storage;

namespace SyncWorkflow.DependencyInjection;

public static class ServiceProviderExtensions
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