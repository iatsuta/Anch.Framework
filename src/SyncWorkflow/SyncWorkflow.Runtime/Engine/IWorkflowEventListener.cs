using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Engine;

public interface IWorkflowEventListener
{
    void OnCurrentStateChanged(WorkflowInstance workflowInstance);
}