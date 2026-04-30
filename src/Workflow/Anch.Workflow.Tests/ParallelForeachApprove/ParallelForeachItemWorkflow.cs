using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;
using Anch.Workflow.States;

namespace Anch.Workflow.Tests.ParallelForeachApprove;

public class ParallelForeachApproveItemWorkflow : BuildWorkflow<ParallelForeachApproveItemWorkflowObject>
{
    public static readonly EventHeader ApproveWaitEvent = new(nameof(ApproveWaitEvent));

    public static readonly EventHeader RejectWaitEvent = new(nameof(RejectWaitEvent));

    protected override void Build(IWorkflowBuilder<ParallelForeachApproveItemWorkflowObject> builder)
    {
        builder
            .Then(wfObj => wfObj.Status = ParallelForeachApproveStatus.Approving)
            .Parallel(

                approveBranch => approveBranch
                    .Then<WaitEventState>()
                    .Input(s => s.Event, ApproveWaitEvent)
                    .Output(wfObj => wfObj.Status, ParallelForeachApproveStatus.Approved),


                rejectBranch => rejectBranch
                    .Then<WaitEventState>()
                    .Input(s => s.Event, RejectWaitEvent)
                    .Output(wfObj => wfObj.Status, ParallelForeachApproveStatus.Rejected))

            .SetBreak(StateBreakPolicy.WaitAny);
    }
}