using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.ExecutionResult;

public record WaitEventResult(EventHeader Event, WorkflowInstance? SourceWorkflow, object? Data = null) : IExecutionResult
{
    public WaitEventInfo ToEventInfo(StateInstance targetState)
    {
        return new WaitEventInfo(this.Event, this.SourceWorkflow, targetState, this.Data);
    }

    public bool LeaveState { get; } = false;
}