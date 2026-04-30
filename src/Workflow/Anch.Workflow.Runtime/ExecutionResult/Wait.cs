namespace Anch.Workflow.ExecutionResult;

public record Wait : IExecutionResult
{
    public bool LeaveState { get; } = false;
}