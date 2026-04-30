using Anch.Workflow.Builder;
using Anch.Workflow.Builder.Default;

namespace Anch.Workflow.Tests.Condition;

public class ConditionWorkflow : BuildWorkflow<ConditionWorkflowObject>
{
    protected override void Build(IWorkflowBuilder<ConditionWorkflowObject> builder)
    {
        builder
            .If(async (ConditionWorkflowObject obj, ConditionWorkflowService service, CancellationToken _) => service.IsEven(obj),
                trueBranch => trueBranch.Then(obj => obj.Result = BuildResult(obj.Value, true)),
                falseBranch => falseBranch.Then(obj => obj.Result = BuildResult(obj.Value, false)));
    }

    public static string BuildResult(int value, bool isEven)
    {
        var evenOrOdd = isEven ? "even" : "odd";
        return $"{value} is {evenOrOdd}!";
    }
}