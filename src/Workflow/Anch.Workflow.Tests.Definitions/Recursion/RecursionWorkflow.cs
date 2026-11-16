using Anch.Core;
using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;

namespace Anch.Workflow.Tests.Recursion;

public class RecursionWorkflow : BuildWorkflow<RecursionWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<RecursionWorkflowObject, Ignore> builder)
    {
        var start = builder
            .Then(_ => { });

        builder
            .If(wfObj => wfObj.Index < wfObj.Limit,
                trueBuilder => trueBuilder.Then(obj => obj.Result += obj.Index)
                    .Then(obj => obj.Index++)
                    .Then(start)) // Loop

            .Then(obj => obj.Result += obj.ExtraAddToResult);
    }
}