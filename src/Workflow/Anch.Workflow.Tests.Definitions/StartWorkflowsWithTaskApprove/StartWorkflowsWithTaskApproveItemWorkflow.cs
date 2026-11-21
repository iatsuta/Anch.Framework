using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;
using Anch.Workflow.Domain;

namespace Anch.Workflow.Tests.StartWorkflowsWithTaskApprove;

public class StartWorkflowsWithTaskApproveItemWorkflow : BuildWorkflow<StartWorkflowsWithTaskApproveItemWorkflowObject, StartWorkflowsWithTaskApproveStatus>
{
    public static readonly EventHeader ApproveWaitEvent = new(nameof(ApproveWaitEvent));

    public static readonly EventHeader RejectWaitEvent = new(nameof(RejectWaitEvent));

    protected override void Build(IWorkflowBuilder<StartWorkflowsWithTaskApproveItemWorkflowObject, StartWorkflowsWithTaskApproveStatus> builder)
    {
        builder
            .WithStatusProperty(wfObj => wfObj.Status)

            .Task(tb => tb
                .AddCommand(ApproveWaitEvent,
                    approvedBuilder =>
                        approvedBuilder
                            .Finish()
                            .WithStatus(StartWorkflowsWithTaskApproveStatus.Approved))

                .AddCommand(RejectWaitEvent,
                    rejectBuilder =>
                        rejectBuilder
                            .Finish()
                            .WithStatus(StartWorkflowsWithTaskApproveStatus.Rejected)))
            .WithStatus(StartWorkflowsWithTaskApproveStatus.Approving);
    }
}