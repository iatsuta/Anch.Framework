using Anch.Core;
using Anch.Workflow.Building;
using Anch.Workflow.Building.Default;

namespace Anch.Workflow.Tests.Condition;

public class ConditionWorkflow : BuildWorkflow<ConditionWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<ConditionWorkflowObject, Ignore> builder) =>

        builder
            .If((ConditionWorkflowObject obj, ConditionWorkflowService service) => service.IsEven(obj),
                trueBranch => trueBranch.Then(obj => obj.Result = BuildResult(obj.Value, true)),
                falseBranch => falseBranch.Then(obj => obj.Result = BuildResult(obj.Value, false)));

    public static string BuildResult(int value, bool isEven)
    {
        var evenOrOdd = isEven ? "even" : "odd";

        return $"{value} is {evenOrOdd}!";
    }
}