namespace Anch.Workflow.Tests.ParallelForeachApprove;

public class ParallelForeachApproveWorkflowObject
{
    public List<ParallelForeachApproveItemWorkflowObject> Items { get; set; } = [];

    public ParallelForeachApproveStatus Status { get; set; } = ParallelForeachApproveStatus.Draft;
}