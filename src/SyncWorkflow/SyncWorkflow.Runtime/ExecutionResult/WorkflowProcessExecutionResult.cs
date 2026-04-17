using SyncWorkflow.Engine;

namespace SyncWorkflow.ExecutionResult;

public record WorkflowProcessExecutionResult(WorkflowProcessResult WorkflowProcessResult, bool LeaveState) : IExecutionResult
{
    //public bool LeaveState => this.ForceLeaveState ?? !this.WorkflowProcessResult.Unprocessed.Any();
}