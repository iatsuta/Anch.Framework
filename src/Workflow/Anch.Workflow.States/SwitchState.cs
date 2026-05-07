using Anch.Workflow.Domain;
using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class SwitchState<TProperty> : IState
{
    public TProperty Value { get; set; } = default!;

    public IReadOnlyDictionary<TProperty, EventHeader> Cases { get; set; } = null!;

    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        return new PushEventResult(
            this.Cases.GetValueOrDefault(this.Value, DefaultCaseEvent),
            executionContext.StateInstance,
            this.Value);
    }

    public static readonly EventHeader DefaultCaseEvent = new EventHeader(nameof(DefaultCaseEvent));
}