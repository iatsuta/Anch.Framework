namespace Anch.Workflow.Tests.StartWorkflowsWithTaskApprove;

public class StartWorkflowsWithTaskApproveItemWorkflowObject
{
    public virtual Guid Id { get; set; }

    public virtual StartWorkflowsWithTaskApproveWorkflowObject Master { get; set; } = null!;


    public virtual StartWorkflowsWithTaskApproveStatus Status { get; set; } = StartWorkflowsWithTaskApproveStatus.Draft;

    public virtual string Name { get; set; } = null!;

    public override string ToString() => this.Name;
}