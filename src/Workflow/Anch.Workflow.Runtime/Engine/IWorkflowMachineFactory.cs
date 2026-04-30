using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Engine;

public interface IWorkflowMachineFactory
{
    IWorkflowMachine Create(WorkflowInstance wi);
}