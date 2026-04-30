using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;

namespace Anch.Workflow.Tests.ParallelForeach;

public class ParallelForeachWorkflow : BuildWorkflow<ParallelForeachWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<ParallelForeachWorkflowObject> builder)
    {
        builder
            .ParallelForeach(
                wfObj => wfObj.Items,
                iteratorBuilder =>
                    iteratorBuilder
                        .StartWorkflow<ParallelForeachItemWorkflowObject, ParallelForeachItemWorkflow>(pair => pair.Element)
                        .Then(pair => pair.Source.Result += pair.Element.EventValue + pair.Element.Value))

            .Then(wfObj => wfObj.Result += wfObj.ExtraAddToResult);
    }
}