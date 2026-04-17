using SyncWorkflow.Engine;
using SyncWorkflow.ExecutionResult;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.States;

public class SwitchState<TProperty> : IState
{
    public TProperty Value { get; set; } = default!;

    public IReadOnlyDictionary<TProperty, EventHeader> Cases { get; set; } = null!;

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        return new PushEventResult(
            this.Cases.GetValueOrDefault(this.Value, DefaultCaseEvent),
            executionContext.StateInstance,
            this.Value);
    }

    public static readonly EventHeader DefaultCaseEvent = new EventHeader(nameof(DefaultCaseEvent));
}