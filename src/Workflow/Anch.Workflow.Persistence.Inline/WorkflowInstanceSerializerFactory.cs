using Anch.Core;
using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence.Inline;

public class WorkflowInstanceSerializerFactory(IServiceProxyFactory serviceProxyFactory) : IWorkflowInstanceSerializerFactory
{
    public IWorkflowInstanceSerializer<TSource> Create<TSource>(IWorkflowDefinition<TSource> workflowDefinition)
        where TSource : class =>

        serviceProxyFactory.Create<IWorkflowInstanceSerializer<TSource>>(
            typeof(WorkflowInstanceSerializer<,>).MakeGenericType(workflowDefinition.SourceType, workflowDefinition.StatusType),
            workflowDefinition);
}