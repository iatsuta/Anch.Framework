namespace Anch.Workflow.Tests.ParallelForeachApprove;

public class ParallelForeachApproveItemWorkflowObject
{
    public ParallelForeachApproveStatus Status { get; set; } = ParallelForeachApproveStatus.Draft;

    public string Name { get; set; } = null!;

    public override string ToString() => this.Name;
}