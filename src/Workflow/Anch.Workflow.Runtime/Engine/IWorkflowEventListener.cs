using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Engine;

public interface IWorkflowEventListener
{
    void OnCurrentStateChanged(WorkflowInstance workflowInstance);
}