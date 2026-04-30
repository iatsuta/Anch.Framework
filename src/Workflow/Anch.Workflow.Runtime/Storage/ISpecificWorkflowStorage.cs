using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Storage;

public interface ISpecificWorkflowStorage : IWorkflowStorageBase<WorkflowInstanceIdentity, StateInstanceIdentity>
{
    IWorkflow Workflow { get; }
}