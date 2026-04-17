using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Storage.Inline;

public interface IWorkflowInstanceSerializer<in TSource>
{
    WorkflowInstance Deserialize(TSource source);

    void Serialize(WorkflowInstance workflowInstance);
}