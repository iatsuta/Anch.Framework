namespace SyncWorkflow.Engine;

public record WorkflowProcessResult(
    IReadOnlyList<IWorkflowMachine> Started,
    IReadOnlyList<UnprocessWorkflowData> Unprocessed)
{
    public WorkflowProcessResult(Func<Task<WorkflowProcessResult>> getResult)
        : this(Array.Empty<IWorkflowMachine>(),
            [new UnprocessWorkflowData(getResult)])
    {
    }

    public async Task<WorkflowProcessResult> ApplyPolicy(WorkflowExecutionPolicy executionPolicy)
    {
        if (executionPolicy.DeepExecute)
        {
            return await this.ProcessUnprocessed();
        }
        else
        {
            return this;
        }
    }

    public async Task<WorkflowProcessResult> ProcessUnprocessed()
    {
        var currentState = this;

        while (currentState.Unprocessed.Any())
        {
            var nextResult = await currentState.Unprocessed.Select(f => f.GetResult()).Aggregate();

            currentState = nextResult with { Started = currentState.Started.Concat(nextResult.Started).ToList() };
        }

        return currentState;
    }

    public static WorkflowProcessResult operator +(WorkflowProcessResult result1, WorkflowProcessResult result2)
    {
        return new[] { result1, result2 }.Aggregate();
    }

    public static WorkflowProcessResult Empty { get; } = new(Started: [], Unprocessed: []);
}

public record UnprocessWorkflowData(Func<Task<WorkflowProcessResult>> GetResult);