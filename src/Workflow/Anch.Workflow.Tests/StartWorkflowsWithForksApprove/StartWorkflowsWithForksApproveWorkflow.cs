using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;

namespace Anch.Workflow.Tests.StartWorkflowsWithForksApprove;

public class StartWorkflowsWithForksApproveWorkflow : BuildWorkflow<StartWorkflowsWithForksApproveWorkflowObject, StartWorkflowsWithForkApproveStatus>
{
    protected override void Build(IWorkflowBuilder<StartWorkflowsWithForksApproveWorkflowObject, StartWorkflowsWithForkApproveStatus> builder) =>

        builder
            .WithStatusProperty(wfObj => wfObj.Status)

            .StartWorkflows<StartWorkflowsWithForksApproveItemWorkflowObject, StartWorkflowsWithForksApproveItemWorkflow>(wfObj => wfObj.Items)
            .WithStatus(StartWorkflowsWithForkApproveStatus.Approving)

            .SetFinishedBreak(item => item.Status != StartWorkflowsWithForkApproveStatus.Approved)

            .Then(wfObj => wfObj.Status = wfObj.Items.All(subWf => subWf.Status == StartWorkflowsWithForkApproveStatus.Approved)
                ? StartWorkflowsWithForkApproveStatus.Approved
                : StartWorkflowsWithForkApproveStatus.Rejected);
}