using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Execution;

public record WaitEventResult(EventHeader Event, WorkflowInstance? SourceWorkflow, object? Data = null) : ExecutionResult
{
    public WaitEventInfo ToEventInfo(StateInstance targetState)
    {
        return new WaitEventInfo(this.Event, this.SourceWorkflow, targetState, this.Data);
    }

    public override bool LeaveState { get; } = false;
}