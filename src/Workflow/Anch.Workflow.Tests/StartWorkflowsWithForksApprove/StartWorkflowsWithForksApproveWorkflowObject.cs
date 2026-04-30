namespace Anch.Workflow.Tests.StartWorkflowsWithForksApprove;

public class StartWorkflowsWithForksApproveWorkflowObject
{
    public List<StartWorkflowsWithForksApproveItemWorkflowObject> Items { get; set; } = [];

    public StartWorkflowsWithForksApproveStatus Status { get; set; } = StartWorkflowsWithForksApproveStatus.Draft;
}