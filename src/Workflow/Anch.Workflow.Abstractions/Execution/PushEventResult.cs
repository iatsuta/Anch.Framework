using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Execution;

public record PushEventResult(EventHeader @Event, StateInstance? TargetState, object? Data = null) : ExecutionResult
{
    public PushEventInfo ToEventInfo(WorkflowInstance sourceWorkflow) => new(this.Event, sourceWorkflow, this.TargetState, this.Data);

    public override bool LeaveState { get; } = true;
}