namespace Anch.Workflow.Execution;

public abstract record ExecutionResult
{
    public abstract bool LeaveState { get; }

    public WorkflowProcessResult WorkflowProcessResult { get; init; } = WorkflowProcessResult.Empty;
}