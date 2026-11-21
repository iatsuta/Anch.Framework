using Anch.Workflow.Domain;
using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class IfState : IState
{
    public bool Condition { get; set; }

    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        var condition = this.Condition;

        return new PushEventResult(condition ? TrueEvent : FalseEvent, executionContext.StateInstance, condition);
    }

    public static EventHeader TrueEvent { get; } = new(nameof(TrueEvent));

    public static EventHeader FalseEvent { get; } = new(nameof(FalseEvent));
}