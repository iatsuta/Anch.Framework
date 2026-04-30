namespace Anch.Workflow.Tests.StartWorkflowsWithTaskApprove;

public class StartWorkflowsWithTaskApproveWorkflowObject
{
    private Guid id;

    private ICollection<StartWorkflowsWithTaskApproveItemWorkflowObject> items { get; set; } = new List<StartWorkflowsWithTaskApproveItemWorkflowObject>();

    private StartWorkflowsWithTaskApproveStatus status = StartWorkflowsWithTaskApproveStatus.Draft;


    public virtual Guid Id
    {
        get => this.id;
        set => this.id = value;
    }

    public virtual ICollection<StartWorkflowsWithTaskApproveItemWorkflowObject> Items
    {
        get => this.items;
        set => this.items = value;
    }

    public virtual StartWorkflowsWithTaskApproveStatus Status
    {
        get => this.status;
        set => this.status = value;
    }
}