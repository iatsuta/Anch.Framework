using Anch.Core;
using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;

namespace Anch.Workflow.Tests.Chain;

public class ChainWorkflow : BuildWorkflow<ChainWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<ChainWorkflowObject, Ignore> builder) =>

        builder
            .Then(obj => obj.Result += obj.Value1)
            .Then(obj => obj.Result += obj.Value2);
}