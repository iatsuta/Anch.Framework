using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow;

public interface IWorkflowHost
{
    IWorkflowExecutor CreateExecutor(WorkflowExecutionPolicy executionPolicy);
}