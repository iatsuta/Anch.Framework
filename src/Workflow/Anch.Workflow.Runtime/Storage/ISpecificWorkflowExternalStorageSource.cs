using Anch.Workflow.Definition;

namespace Anch.Workflow.Storage;

public interface ISpecificWorkflowExternalStorageSource
{
    IReadOnlyDictionary<WorkflowDefinitionIdentity, ISpecificWorkflowExternalStorage> GetSpecificStorageDict();
}