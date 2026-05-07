using Anch.IdentitySource.DependencyInjection;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Serialization.Inline.IdGenerator;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Serialization.Inline;

public class InlineWorkflowDatabaseProvider : IWorkflowDatabaseProvider
{
    public void AddServices(IServiceCollection services) =>

        services
            .AddIdentitySource()
            .AddScoped<IWorkflowRepositoryFactory, InlineWorkflowRepositoryFactory>()
            .AddScoped<IInstanceIdGenerator<WorkflowInstance>, WorkflowInstanceInlineIdGenerator>()
            .AddScoped<IInstanceIdGenerator<StateInstance>, StateInstanceInlineIdGenerator>()
            .AddScoped<IWorkflowInstanceSerializerFactory, WorkflowInstanceSerializerFactory>()

            .AddScoped<IStateInstanceSerializerFactory, StateInstanceSerializerFactory>()

            .AddSingleton<IStateDefinitionResolverFactory, StateDefinitionResolverFactory>()

            .AddScoped(typeof(IInlineStorage<>), typeof(InlineStorage<>));

    //    .AddScoped<IWorkflowInstanceSerializerFactory, WorkflowInstanceSerializerFactory>()
    //.AddScoped<IStateInstanceSerializerFactory, StateInstanceSerializerFactory>()
    //
}