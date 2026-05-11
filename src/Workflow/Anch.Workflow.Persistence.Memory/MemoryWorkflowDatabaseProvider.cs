using Anch.Workflow.Domain.Runtime;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Persistence.Memory;

public class MemoryWorkflowDatabaseProvider : IWorkflowDatabaseProvider
{
    public void AddServices(IServiceCollection services) =>

        services

            .AddScoped<IInstanceIdGenerator<WorkflowInstance>, MemoryInstanceIdGenerator<WorkflowInstance>>()
            .AddScoped<IInstanceIdGenerator<StateInstance>, MemoryInstanceIdGenerator<StateInstance>>()

            .AddSingleton<MemoryWorkflowRootState>()
            .AddScoped<IWorkflowRepositoryFactory, MemoryWorkflowRepositoryFactory>();
}