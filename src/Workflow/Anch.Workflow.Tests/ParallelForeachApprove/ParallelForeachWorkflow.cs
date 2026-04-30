using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;

namespace Anch.Workflow.Tests.ParallelForeachApprove;

public class ParallelForeachApproveWorkflow : BuildWorkflow<ParallelForeachApproveWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<ParallelForeachApproveWorkflowObject> builder)
    {
        builder
            .ParallelForeach(
                wfObj => wfObj.Items,
                iteratorBuilder =>
                    iteratorBuilder
                        .StartWorkflow<ParallelForeachApproveItemWorkflowObject, ParallelForeachApproveItemWorkflow>(pair => pair.Element))
            .SetFinishedBreak(item => item.Status != ParallelForeachApproveStatus.Approved)
            .WithStatus(ParallelForeachApproveStatus.Approving)
            
            .If(wfObj => wfObj.Items.All(subWf => subWf.Status == ParallelForeachApproveStatus.Approved),
                trueBranch => trueBranch.Finish().WithStatus(ParallelForeachApproveStatus.Approved),
                falseBranch => falseBranch.Finish().WithStatus(ParallelForeachApproveStatus.Rejected));
    }
}