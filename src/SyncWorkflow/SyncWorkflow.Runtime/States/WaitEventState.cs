using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.Engine;
using SyncWorkflow.ExecutionResult;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.States;

public class WaitEventState : IState
{
    public EventHeader Event { get; set; } = null!;

    public WorkflowInstance? SourceWorkflow { get; set; }

    public object? ExpectedData { get; set; }

    public object? ReceivedData { get; set; }

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        if (executionContext.IsCallbackEvent)
        {
            this.ReceivedData = executionContext.CallbackEventInfo!.Data;

            return new Done();
        }

        return new WaitEventResult(this.Event, this.SourceWorkflow, this.ExpectedData);
    }
}

public class WaitEventState<TSource, TData> : IState
{
    public EventHeader Event { get; set; } = null!;

    public WorkflowInstance? SourceWorkflow { get; set; }

    public Func<TSource, TData, CancellationToken, Task>? Callback { get; set; }
    
    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        if (executionContext.IsCallbackEvent)
        {
            this.ReceivedData = executionContext.CallbackEventInfo!.Data;

            return new Done();
        }

        return new WaitEventResult(this.Event, this.SourceWorkflow);
    }
}