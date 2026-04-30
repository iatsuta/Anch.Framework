using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.ExecutionResult;

public record WaitEventResult(EventHeader Event, WorkflowInstance? SourceWorkflow, object? Data = null) : IExecutionResult
{
    public WaitEventInfo ToEventInfo(StateInstance targetState)
    {
        return new WaitEventInfo(this.Event, this.SourceWorkflow, targetState, this.Data);
    }

    public bool LeaveState { get; } = false;
}