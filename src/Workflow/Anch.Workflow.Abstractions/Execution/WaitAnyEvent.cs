namespace Anch.Workflow.Execution;

public record WaitAnyEvent : ExecutionResult
{
    public override bool LeaveState { get; } = false;
}