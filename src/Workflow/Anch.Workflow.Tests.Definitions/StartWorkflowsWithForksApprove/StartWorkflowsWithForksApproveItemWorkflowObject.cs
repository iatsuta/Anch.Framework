namespace Anch.Workflow.Tests.StartWorkflowsWithForksApprove;

public class StartWorkflowsWithForksApproveItemWorkflowObject
{
    public StartWorkflowsWithForkApproveStatus Status { get; set; } = StartWorkflowsWithForkApproveStatus.Draft;

    public string Name { get; set; } = null!;

    public override string ToString() => this.Name;
}