using Anch.Core;
using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;

namespace Anch.Workflow.Tests.Sum;

public class SumWorkflow : BuildWorkflow<SumWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<SumWorkflowObject, Ignore> builder) =>

        builder.Then(obj => obj.Result = obj.Value1 + obj.Value2);
}