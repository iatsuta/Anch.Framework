using Anch.Workflow.Definition;

namespace Anch.Workflow.Storage;

public interface IWorkflowSource
{
    IReadOnlyDictionary<WorkflowDefinitionIdentity, IWorkflow> GetWorkflows();
}