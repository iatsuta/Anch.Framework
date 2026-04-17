namespace SyncWorkflow.ExecutionResult;

public record MultiExecutionResult(IReadOnlyCollection<IExecutionResult> ExecutionResults) : IExecutionResult
{
    public MultiExecutionResult(IEnumerable<IExecutionResult> executionResults)
        : this(executionResults.ToList())
    {
    }

    public MultiExecutionResult(params IExecutionResult[] executionResults)
        : this((IEnumerable<IExecutionResult>)executionResults)
    {
    }

    public bool LeaveState => this.ExecutionResults.All(er => er.LeaveState);
}