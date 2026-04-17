using SyncWorkflow.Domain.Definition;
using SyncWorkflow.Engine;

namespace SyncWorkflow.Domain.Runtime;

public class WorkflowInstance
{
    public StateInstance? Owner { get; set; }

    public required IWorkflowDefinition Definition { get; init; }

    public required object Source { get; init; }

    public Guid Id { get; set; }

    public WorkflowInstanceFullIdentity Identity => new (this.Id, this.Definition.Identity);

    public StateInstance CurrentState { get; set; } = null!;

    public required WorkflowStatus Status { get; set; }

    public void SetStatus(WorkflowStatus workflowStatus)
    {
        if (this.Status.Role == WorkflowStatusRole.Finished)
        {
            throw new InvalidOperationException("Workflow already finished.");
        }

        this.Status = workflowStatus;
    }
}