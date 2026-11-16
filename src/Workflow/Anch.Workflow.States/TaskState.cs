using Anch.Workflow.Domain;
using Anch.Workflow.Engine;
using Anch.Workflow.Execution;

namespace Anch.Workflow.States;

public class TaskState : IState
{
    public IReadOnlyList<EventHeader> CommandHeaders { get; set; } = null!;

    public async ValueTask<ExecutionResult> Run(IExecutionContext executionContext)
    {
        if (executionContext.CallbackEventInfo is { } callbackEventInfo)
        {
            executionContext.StateInstance.ReleaseWaitEvents();

            return new PushEventResult(callbackEventInfo.Header, null, callbackEventInfo.Data);
        }
        else
        {
            return new MultiExecutionResult([.. this.CommandHeaders.Select(header => new WaitEventResult(header, null))]);
        }
    }

    public static readonly string CommandsKey = "Commands";
}