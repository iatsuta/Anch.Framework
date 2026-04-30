namespace Anch.Workflow.Tests.StartWorkflowsWithForksApprove;

public class StartWorkflowsWithForksApproveItemWorkflowObject
{
    public StartWorkflowsWithForksApproveStatus Status { get; set; } = StartWorkflowsWithForksApproveStatus.Draft;

    public string Name { get; set; } = null!;

    public override string ToString()
    {
        return this.Name;
    }
}