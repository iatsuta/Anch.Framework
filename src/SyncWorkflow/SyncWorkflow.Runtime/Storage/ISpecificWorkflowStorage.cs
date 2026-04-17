using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Storage;

public interface ISpecificWorkflowStorage : IWorkflowStorageBase<WorkflowInstanceIdentity, StateInstanceIdentity>
{
    IWorkflow Workflow { get; }
}