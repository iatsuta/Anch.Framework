using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Storage.Inline;

public interface IWorkflowInstanceSerializer<in TSource>
{
    WorkflowInstance Deserialize(TSource source);

    void Serialize(WorkflowInstance workflowInstance);
}