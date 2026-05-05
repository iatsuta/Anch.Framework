using Anch.Core;
using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;
using Anch.Workflow.Domain;

namespace Anch.Workflow.Tests.StartWorkflowsWithTaskApprove;

public class StartWorkflowsWithTaskApproveItemWorkflow : BuildWorkflow<StartWorkflowsWithTaskApproveItemWorkflowObject>
{
    public static readonly EventHeader ApproveWaitEvent = new(nameof(ApproveWaitEvent));

    public static readonly EventHeader RejectWaitEvent = new(nameof(RejectWaitEvent));

    protected override void Build(IWorkflowBuilder<StartWorkflowsWithTaskApproveItemWorkflowObject, Ignore> builder)
    {
        builder
            .Then(wfObj => wfObj.Status = StartWorkflowsWithTaskApproveStatus.Approving)

            .Task(tb => tb
                .AddCommand(ApproveWaitEvent,
                    approvedBuilder =>
                        approvedBuilder.Then(wfObj => wfObj.Status = StartWorkflowsWithTaskApproveStatus.Approved))

                .AddCommand(RejectWaitEvent,
                    rejectBuilder =>
                        rejectBuilder.Then(wfObj => wfObj.Status = StartWorkflowsWithTaskApproveStatus.Rejected)));
    }
}