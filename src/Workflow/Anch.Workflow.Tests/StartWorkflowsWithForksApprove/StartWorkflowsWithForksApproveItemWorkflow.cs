using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;
using Anch.Workflow.Domain;
using Anch.Workflow.States;

namespace Anch.Workflow.Tests.StartWorkflowsWithForksApprove;

public class StartWorkflowsWithForksApproveItemWorkflow : BuildWorkflow<StartWorkflowsWithForksApproveItemWorkflowObject, StartWorkflowsWithForkApproveStatus>
{
    public static readonly EventHeader ApproveWaitEvent = new(nameof(ApproveWaitEvent));

    public static readonly EventHeader RejectWaitEvent = new(nameof(RejectWaitEvent));

    protected override void Build(IWorkflowBuilder<StartWorkflowsWithForksApproveItemWorkflowObject, StartWorkflowsWithForkApproveStatus> builder)
    {
        builder
            .WithStatusProperty(wfObj => wfObj.Status)

            .Parallel(
                approveBranch => approveBranch
                    .Then<WaitEventState>()
                    .Input(s => s.Event, ApproveWaitEvent)
                    .Finish()
                    .WithStatus(StartWorkflowsWithForkApproveStatus.Approved),

                rejectBranch => rejectBranch
                    .Then<WaitEventState>()
                    .Input(s => s.Event, RejectWaitEvent)
                    .Finish()
                    .WithStatus(StartWorkflowsWithForkApproveStatus.Rejected))

            .SetBreak(StateBreakPolicy.WaitAny)
            .WithStatus(StartWorkflowsWithForkApproveStatus.Approving);
    }
}