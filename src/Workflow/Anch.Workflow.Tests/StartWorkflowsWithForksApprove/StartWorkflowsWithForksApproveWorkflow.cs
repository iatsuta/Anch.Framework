using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;

namespace Anch.Workflow.Tests.StartWorkflowsWithForksApprove;

public class StartWorkflowsWithForksApproveWorkflow : BuildWorkflow<StartWorkflowsWithForksApproveWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<StartWorkflowsWithForksApproveWorkflowObject> builder)
    {
        builder
            .Then(wfObj => wfObj.Status = StartWorkflowsWithForksApproveStatus.Approving)

            .StartWorkflows<StartWorkflowsWithForksApproveItemWorkflowObject, StartWorkflowsWithForksApproveItemWorkflow>(wfObj => wfObj.Items)

            .SetFinishedBreak(item => item.Status != StartWorkflowsWithForksApproveStatus.Approved)

            .Then(wfObj => wfObj.Status = wfObj.Items.All(subWf => subWf.Status == StartWorkflowsWithForksApproveStatus.Approved)
                                         ? StartWorkflowsWithForksApproveStatus.Approved
                                         : StartWorkflowsWithForksApproveStatus.Rejected);
    }
}