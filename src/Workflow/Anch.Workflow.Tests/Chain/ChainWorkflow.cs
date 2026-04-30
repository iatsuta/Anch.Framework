using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;

namespace Anch.Workflow.Tests.Chain;

public class ChainWorkflow : BuildWorkflow<ChainWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<ChainWorkflowObject> builder)
    {
        builder
            .Then(obj => obj.Result += obj.Value1)
            .Then(obj => obj.Result += obj.Value2);
    }
}