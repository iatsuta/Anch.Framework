using Anch.Core;
using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;

namespace Anch.Workflow.Tests.ParallelForeach;

public class ParallelForeachWorkflow : BuildWorkflow<ParallelForeachWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<ParallelForeachWorkflowObject, Ignore> builder) =>

        builder
            .ParallelForeach(
                wfObj => wfObj.Items,
                iteratorBuilder =>
                    iteratorBuilder
                        .StartWorkflow<ParallelForeachItemWorkflowObject, ParallelForeachItemWorkflow>(pair => pair.Element)
                        .Then(pair => pair.Source.Result += pair.Element.EventValue + pair.Element.Value))

            .Then(wfObj => wfObj.Result += wfObj.ExtraAddToResult);
}