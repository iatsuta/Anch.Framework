using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;
using Anch.Workflow.Domain;
using Anch.Workflow.States;

namespace Anch.Workflow.Tests.Parallel;

public class ParallelWorkflow : BuildWorkflow<ParallelWorkflowObject, ParallelApproveStatus>
{
    public static readonly EventHeader ApproveWaitEvent = new(nameof(ApproveWaitEvent));

    public static readonly EventHeader RejectWaitEvent = new(nameof(RejectWaitEvent));

    protected override void Build(IWorkflowBuilder<ParallelWorkflowObject, ParallelApproveStatus> builder) =>

        builder
            .WithStatusProperty(wfObj => wfObj.Status)

            .Parallel(
                approveBranch => approveBranch
                    .Then<WaitEventState>()
                    .Input(s => s.Event, ApproveWaitEvent)
                    .Finish()
                    .WithStatus(ParallelApproveStatus.Approved),

                rejectBranch => rejectBranch
                    .Then<WaitEventState>()
                    .Input(s => s.Event, RejectWaitEvent)
                    .Finish()
                    .WithStatus(ParallelApproveStatus.Rejected))

            .SetBreak(StateBreakPolicy.WaitAny)
            .WithStatus(ParallelApproveStatus.Approving);
}