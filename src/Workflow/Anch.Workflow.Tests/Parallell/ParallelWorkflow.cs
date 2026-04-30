using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;
using Anch.Workflow.States;

namespace Anch.Workflow.Tests.Parallell;

public class ParallelWorkflow : BuildWorkflow<ParallelWorkflowObject>
{
    public static readonly EventHeader ApproveWaitEvent = new(nameof(ApproveWaitEvent));

    public static readonly EventHeader RejectWaitEvent = new(nameof(RejectWaitEvent));

    protected override void Build(IWorkflowBuilder<ParallelWorkflowObject> builder)
    {
        builder
            .Then(wfObj => wfObj.Status = ParallelApproveStatus.Approving)
            .Parallel(

                approveBranch => approveBranch
                    .Then<WaitEventState>()
                    .Input(s => s.Event, ApproveWaitEvent)
                    .Output(wfObj => wfObj.Status, ParallelApproveStatus.Approved),


                rejectBranch => rejectBranch
                    .Then<WaitEventState>()
                    .Input(s => s.Event, RejectWaitEvent)
                    .Output(wfObj => wfObj.Status, ParallelApproveStatus.Rejected))

            .SetBreak(StateBreakPolicy.WaitAny);
    }
}