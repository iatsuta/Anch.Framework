namespace Anch.Workflow.Execution;

public record Done : ExecutionResult
{
    public override bool LeaveState { get; } = true;
}