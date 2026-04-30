namespace Anch.Workflow.Tests.TaskWorkflow;

public class TaskWorkflowObject
{
    private Guid id;

    private TaskApproveStatus status = TaskApproveStatus.Draft;

    private bool postProcessWork;


    public virtual Guid Id
    {
        get => this.id;
        set => this.id = value;
    }

    public virtual TaskApproveStatus Status
    {
        get => this.status;
        set => this.status = value;
    }

    public virtual bool PostProcessWork
    {
        get => this.postProcessWork;
        set => this.postProcessWork = value;
    }
}