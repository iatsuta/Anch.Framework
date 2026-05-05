using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;
using Anch.Workflow.Domain;

namespace Anch.Workflow.Tests.TaskWorkflow;

public class TaskWorkflow : BuildWorkflow<TaskWorkflowObject, TaskApproveStatus>
{
    public static readonly EventHeader ApproveEventHeader = new("Approve");

    public static readonly EventHeader RejectEventHeader = new("Reject");


    protected override void Build(IWorkflowBuilder<TaskWorkflowObject, TaskApproveStatus> builder) =>

        builder
            .Then(wfObj => wfObj.Status = TaskApproveStatus.Approving)
            .WithName("Start")

            .Task(tb => tb
                .AddCommand(ApproveEventHeader,
                    approveBranch => approveBranch
                        .Then(wfObj => wfObj.Status = TaskApproveStatus.Approved)
                        .WithStatus(TaskApproveStatus.Approved))

                .AddCommand(RejectEventHeader,
                    rejectBranch => rejectBranch
                        .Then(wfObj => wfObj.Status = TaskApproveStatus.Rejected)
                        .WithStatus(TaskApproveStatus.Rejected)))

            .WithStatus(TaskApproveStatus.Approving)

            .Then(wfObj => wfObj.PostProcessWork = true)
            .WithName("PostProcessWorkState");
}