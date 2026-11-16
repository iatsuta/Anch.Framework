using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence.Inline;

public interface IStateInstanceSerializerFactory<TSource, TStatus>
    where TSource : class
    where TStatus : struct
{
    IStateInstanceSerializer Create(IWorkflowDefinition<TSource, TStatus> workflowDefinition);
}