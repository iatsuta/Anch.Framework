using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;

namespace Anch.Workflow.Tests.StartWorkflowsWithTaskApprove;

public class StartWorkflowsWithTaskApproveWorkflow : BuildWorkflow<StartWorkflowsWithTaskApproveWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<StartWorkflowsWithTaskApproveWorkflowObject> builder)
    {
        builder
            .Then(wfObj => wfObj.Status = StartWorkflowsWithTaskApproveStatus.Approving)

            .StartWorkflows<StartWorkflowsWithTaskApproveItemWorkflowObject, StartWorkflowsWithTaskApproveItemWorkflow>(wfObj => wfObj.Items)

            .SetFinishedBreak(item => item.Status != StartWorkflowsWithTaskApproveStatus.Approved)

            .Then(wfObj => wfObj.Status = wfObj.Items.All(subWf => subWf.Status == StartWorkflowsWithTaskApproveStatus.Approved)
                                         ? StartWorkflowsWithTaskApproveStatus.Approved
                                         : StartWorkflowsWithTaskApproveStatus.Rejected);
    }
}