using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Storage;

public interface IWorkflowSource
{
    IReadOnlyDictionary<WorkflowDefinitionIdentity, IWorkflow> GetWorkflows();
}