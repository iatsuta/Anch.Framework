using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Domain.Runtime;

public class WorkflowInstance
{
    public StateInstance? Owner { get; set; }

    public required IWorkflowDefinition Definition { get; init; }

    public required object Source { get; init; }

    public Guid Id { get; set; }

    public WorkflowInstanceIdentity Identity => new(this.Id, this.Definition.Identity);

    public StateInstance CurrentState { get; set; } = null!;

    public required WorkflowStatus Status
    {
        get;
        set
        {
            if (field?.Role == WorkflowStatusRole.Finished)
            {
                throw new InvalidOperationException("Workflow already finished.");
            }

            field = value;
        }
    }
}