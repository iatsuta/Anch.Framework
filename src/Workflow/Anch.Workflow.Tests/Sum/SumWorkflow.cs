using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;

namespace Anch.Workflow.Tests.Sum;

public class SumWorkflow : BuildWorkflow<SumWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<SumWorkflowObject> builder)
    {
        builder.Then(obj => obj.Result = obj.Value1 + obj.Value2);
    }
}