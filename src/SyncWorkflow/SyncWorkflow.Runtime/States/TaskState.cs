using SyncWorkflow.Engine;
using SyncWorkflow.ExecutionResult;
using SyncWorkflow.States._Base;

namespace SyncWorkflow.States;

public class TaskState : IState
{
    public IReadOnlyList<EventHeader> CommandHeaders { get; set; } = null!;

    public async Task<IExecutionResult> Run(IExecutionContext executionContext)
    {
        if (executionContext.IsCallbackEvent)
        {
            executionContext.StateInstance.ReleaseWaitEvents();

            return new PushEventResult(executionContext.CallbackEventInfo!.Event, null, executionContext.CallbackEventInfo!.Data);
        }
        else
        {
            return new MultiExecutionResult(this.CommandHeaders.Select(header => new WaitEventResult(header, null)));
        }
    }

    public static readonly string CommandsKey = "Commands";
}