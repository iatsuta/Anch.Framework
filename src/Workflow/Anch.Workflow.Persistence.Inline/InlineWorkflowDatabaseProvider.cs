using Anch.IdentitySource.DependencyInjection;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Persistence.Inline.IdGenerator;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Persistence.Inline;

public class InlineWorkflowDatabaseProvider : IWorkflowDatabaseProvider
{
    public void AddServices(IServiceCollection services) =>

        services
            .AddIdentitySource()
            .AddScoped<IWorkflowRepositoryFactory, InlineWorkflowRepositoryFactory>()
            .AddScoped<IInstanceIdGenerator<WorkflowInstance>, WorkflowInstanceInlineIdGenerator>()
            .AddScoped<IInstanceIdGenerator<StateInstance>, StateInstanceInlineIdGenerator>()
            .AddScoped<IWorkflowInstanceSerializerFactory, WorkflowInstanceSerializerFactory>()

            .AddScoped(typeof(IStateInstanceSerializerFactory<,>), typeof(StateInstanceSerializerFactory<,>))

            .AddSingleton<IStateDefinitionResolverFactory, StateDefinitionResolverFactory>()

            .AddScoped(typeof(IInlineStorage<>), typeof(InlineStorage<>));

    //    .AddScoped<IWorkflowInstanceSerializerFactory, WorkflowInstanceSerializerFactory>()
    //.AddScoped<IStateInstanceSerializerFactory, StateInstanceSerializerFactory>()
    //
}