using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Domain.Runtime;

public record StateInstanceIdentity(Guid Id)
{
    public StateInstanceFullIdentity ToFull(WorkflowDefinitionIdentity workflow) => new(this.Id, workflow);
}

public record StateInstanceFullIdentity(Guid Id, WorkflowDefinitionIdentity Workflow) : StateInstanceIdentity(Id);