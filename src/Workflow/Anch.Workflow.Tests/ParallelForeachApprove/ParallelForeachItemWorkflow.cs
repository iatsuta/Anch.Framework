using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;
using Anch.Workflow.Domain;
using Anch.Workflow.States;

namespace Anch.Workflow.Tests.ParallelForeachApprove;

public class ParallelForeachApproveItemWorkflow : BuildWorkflow<ParallelForeachApproveItemWorkflowObject, ParallelForeachApproveStatus>
{
    public static readonly EventHeader ApproveWaitEvent = new(nameof(ApproveWaitEvent));

    public static readonly EventHeader RejectWaitEvent = new(nameof(RejectWaitEvent));

    protected override void Build(IWorkflowBuilder<ParallelForeachApproveItemWorkflowObject, ParallelForeachApproveStatus> builder) =>

        builder
            .WithStatusProperty(wfObj => wfObj.Status)

            .Parallel(
                approveBranch => approveBranch
                    .Then<WaitEventState>()
                    .Input(s => s.Event, ApproveWaitEvent)
                    .Finish()
                    .WithStatus(ParallelForeachApproveStatus.Approved),

                rejectBranch => rejectBranch
                    .Then<WaitEventState>()
                    .Input(s => s.Event, RejectWaitEvent)
                    .Finish()
                    .WithStatus(ParallelForeachApproveStatus.Rejected))

            .SetBreak(StateBreakPolicy.WaitAny)
            .WithStatus(ParallelForeachApproveStatus.Approving);
}