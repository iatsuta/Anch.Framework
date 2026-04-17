using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Domain.Runtime;

public record WorkflowInstanceIdentity(Guid Id)
{
    public WorkflowInstanceFullIdentity ToFull(WorkflowDefinitionIdentity workflow) => new (this.Id, workflow);
}

public record WorkflowInstanceFullIdentity(Guid Id, WorkflowDefinitionIdentity Workflow) : WorkflowInstanceIdentity(Id);