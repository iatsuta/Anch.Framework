namespace Anch.Workflow.Tests.StartWorkflowsWithTaskApprove;

public class StartWorkflowsWithTaskApproveItemWorkflowObject
{
    private Guid id;

    private string name = null!;

    private StartWorkflowsWithTaskApproveStatus status = StartWorkflowsWithTaskApproveStatus.Draft;

    private StartWorkflowsWithTaskApproveWorkflowObject master;

    public virtual Guid Id
    {
        get => this.id;
        set => this.id = value;
    }

    public virtual StartWorkflowsWithTaskApproveWorkflowObject Master
    {
        get => this.master;
        set => this.master = value;
    }


    public virtual StartWorkflowsWithTaskApproveStatus Status
    {
        get => this.status;
        set => this.status = value;
    }

    public virtual string Name
    {
        get => this.name;
        set => this.name = value;
    }

    public override string ToString()
    {
        return this.Name;
    }
}