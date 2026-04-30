using Anch.Workflow.Definition;

namespace Anch.Workflow.Storage;

public interface ISpecificWorkflowStorageSource
{
    IReadOnlyDictionary<WorkflowDefinitionIdentity, ISpecificWorkflowStorage> GetSpecificStorageDict();
}