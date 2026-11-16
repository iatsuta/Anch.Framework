namespace Anch.Workflow.Tests.TaskWorkflow;

public class TaskWorkflowObject
{
    public virtual Guid Id { get; set; }

    public virtual TaskApproveStatus Status { get; set; } = TaskApproveStatus.Draft;

    public virtual bool PostProcessWork { get; set; }
}