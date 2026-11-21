using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence.Inline;

public interface IWorkflowInstanceSerializerFactory
{
    IWorkflowInstanceSerializer<TSource> Create<TSource>(IWorkflowDefinition<TSource> workflowDefinition)
        where TSource : class;
}