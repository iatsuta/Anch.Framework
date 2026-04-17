using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.Engine;
using SyncWorkflow.ExecutionResult;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.States;

public class PushEventState : IState
{
    public EventHeader Event { get; set; } = null!;

    public StateInstance? TargetState { get; set; }

    public object? Data { get; set; }

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        if (executionContext.IsCallbackEvent)
        {
            return new Done();
        }

        return new PushEventResult(this.Event, this.TargetState, this.Data);
    }
}