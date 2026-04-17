using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Storage;

public interface ISpecificWorkflowExternalStorageSource
{
    IReadOnlyDictionary<WorkflowDefinitionIdentity, ISpecificWorkflowExternalStorage> GetSpecificStorageDict();
}