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
            .WithStatusProperty(wfObj => wfObj.Status)

            .Task(tb => tb
                .AddCommand(ApproveEventHeader,
                    approveBranch => approveBranch

                        .Then(wfObj => wfObj.PostProcessWork = true)
                        .WithName("PostProcessWorkState")

                        .Finish()
                        .WithStatus(TaskApproveStatus.Approved))

                .AddCommand(RejectEventHeader,
                    rejectBranch => rejectBranch

                        .Finish()
                        .WithStatus(TaskApproveStatus.Rejected)))

            .WithStatus(TaskApproveStatus.Approving);
}