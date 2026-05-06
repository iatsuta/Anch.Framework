using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;

namespace Anch.Workflow.Tests.StartWorkflowsWithTaskApprove;

public class StartWorkflowsWithTaskApproveWorkflow : BuildWorkflow<StartWorkflowsWithTaskApproveWorkflowObject, StartWorkflowsWithTaskApproveStatus>
{
    protected override void Build(IWorkflowBuilder<StartWorkflowsWithTaskApproveWorkflowObject, StartWorkflowsWithTaskApproveStatus> builder) =>

        builder
            .WithStatusProperty(wfObj => wfObj.Status)

            .StartWorkflows<StartWorkflowsWithTaskApproveItemWorkflowObject, StartWorkflowsWithTaskApproveItemWorkflow>(wfObj => wfObj.Items)
            .SetFinishedBreak(item => item.Status != StartWorkflowsWithTaskApproveStatus.Approved)
            .WithStatus(StartWorkflowsWithTaskApproveStatus.Approving)

            .Then(wfObj => wfObj.Status = wfObj.Items.All(subWf => subWf.Status == StartWorkflowsWithTaskApproveStatus.Approved)
                ? StartWorkflowsWithTaskApproveStatus.Approved
                : StartWorkflowsWithTaskApproveStatus.Rejected);
}