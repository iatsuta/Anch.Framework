namespace SyncWorkflow.Engine;

public static class WorkflowProcessResultExtensions
{
    public static WorkflowProcessResult Aggregate(this IEnumerable<WorkflowProcessResult> workflowProcessResults)
    {
        var started = new List<IWorkflowMachine>();
        var unprocessed = new List<UnprocessWorkflowData>();

        foreach (var workflowProcessResult in workflowProcessResults)
        {
            started.AddRange(workflowProcessResult.Started);
            unprocessed.AddRange(workflowProcessResult.Unprocessed);
        }

        return new WorkflowProcessResult(started, unprocessed);
    }

    public static async Task<WorkflowProcessResult> Aggregate(this IEnumerable<Task<WorkflowProcessResult>> getResults)
    {
        var started = new List<IWorkflowMachine>();
        var unprocessed = new List<UnprocessWorkflowData>();

        foreach (var getResult in getResults)
        {
            var workflowProcessResult = await getResult;

            started.AddRange(workflowProcessResult.Started);
            unprocessed.AddRange(workflowProcessResult.Unprocessed);
        }

        return new WorkflowProcessResult(started, unprocessed);
    }

    public static async Task<WorkflowProcessResult> ApplyPolicy(this Task<WorkflowProcessResult> asyncWorkflowProcessResult, WorkflowExecutionPolicy executionPolicy)
    {
        var workflowProcessResult = await asyncWorkflowProcessResult;

        return await workflowProcessResult.ApplyPolicy(executionPolicy);
    }
}