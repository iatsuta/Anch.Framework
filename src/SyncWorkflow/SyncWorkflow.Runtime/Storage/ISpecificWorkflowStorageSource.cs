using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Storage;

public interface ISpecificWorkflowStorageSource
{
    IReadOnlyDictionary<WorkflowDefinitionIdentity, ISpecificWorkflowStorage> GetSpecificStorageDict();
}