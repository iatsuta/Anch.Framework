using Anch.Workflow.Definition;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Storage;

public interface IWorkflowStorage : IWorkflowStorageBase<WorkflowInstanceFullIdentity, StateInstanceFullIdentity>
{
    ISpecificWorkflowStorage GetSpecificStorage(WorkflowDefinitionIdentity identity);
}