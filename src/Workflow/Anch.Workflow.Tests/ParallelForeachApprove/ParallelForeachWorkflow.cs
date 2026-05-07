using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;

namespace Anch.Workflow.Tests.ParallelForeachApprove;

public class ParallelForeachApproveWorkflow : BuildWorkflow<ParallelForeachApproveWorkflowObject, ParallelForeachApproveStatus>
{
    protected override void Build(IWorkflowBuilder<ParallelForeachApproveWorkflowObject, ParallelForeachApproveStatus> builder) =>

        builder
            .WithStatusProperty(wfObj => wfObj.Status)
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