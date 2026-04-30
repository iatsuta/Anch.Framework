using Anch.Workflow.Engine;
using Anch.Workflow.ExecutionResult;
using Anch.Workflow.States._Base;

namespace Anch.Workflow.States;

public class IfState : IState
{
    public bool Condition { get; set; }

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        var condition = this.Condition;

        return new PushEventResult(condition ? TrueEvent : FalseEvent, executionContext.StateInstance, condition);
    }

    public static EventHeader TrueEvent { get; } = new(nameof(TrueEvent));

    public static EventHeader FalseEvent { get; } = new(nameof(FalseEvent));
}