namespace Anch.Workflow.Tests.StartWorkflowsWithForksApprove;

public class StartWorkflowsWithForksApproveWorkflowObject
{
    public List<StartWorkflowsWithForksApproveItemWorkflowObject> Items { get; set; } = [];

    public StartWorkflowsWithForkApproveStatus Status { get; set; } = StartWorkflowsWithForkApproveStatus.Draft;
}