using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Engine;

public interface IExecutionContext
{
    StateInstance StateInstance { get; }

    CancellationToken CancellationToken { get; }

    WaitEventInfo? CallbackEventInfo { get; }



    bool IsCallbackEvent => this.CallbackEventInfo != null;

    object Source { get; }

    WorkflowInstance WorkflowInstance { get; }

    bool IsActual { get; }
}