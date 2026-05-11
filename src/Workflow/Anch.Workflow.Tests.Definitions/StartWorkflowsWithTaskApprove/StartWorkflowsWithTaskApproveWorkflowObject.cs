namespace Anch.Workflow.Tests.StartWorkflowsWithTaskApprove;

public class StartWorkflowsWithTaskApproveWorkflowObject
{
    public virtual Guid Id { get; set; }

    public virtual ICollection<StartWorkflowsWithTaskApproveItemWorkflowObject> Items { get; set; } = [];

    public virtual StartWorkflowsWithTaskApproveStatus Status { get; set; } = StartWorkflowsWithTaskApproveStatus.Draft;
}