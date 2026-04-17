using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.ExecutionResult;

public record PushEventResult(EventHeader @Event, StateInstance? TargetState, object? Data = null) : IExecutionResult
{
    public PushEventInfo ToEventInfo(WorkflowInstance sourceWorkflow)
    {
        return new PushEventInfo(this.Event, sourceWorkflow, this.TargetState, this.Data);
    }

    public bool LeaveState { get; } = true;
}