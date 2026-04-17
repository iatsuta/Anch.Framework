using SyncWorkflow.Domain.Definition;
using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Storage;

public interface IWorkflowStorage : IWorkflowStorageBase<WorkflowInstanceFullIdentity, StateInstanceFullIdentity>
{
    ISpecificWorkflowStorage GetSpecificStorage(WorkflowDefinitionIdentity identity);
}