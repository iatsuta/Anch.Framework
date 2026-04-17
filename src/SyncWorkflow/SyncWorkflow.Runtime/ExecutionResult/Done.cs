namespace SyncWorkflow.ExecutionResult;

public record Done : IExecutionResult
{
    public bool LeaveState { get; } = true;
}