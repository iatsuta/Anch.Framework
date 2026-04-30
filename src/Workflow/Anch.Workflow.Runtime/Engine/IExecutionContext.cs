using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Engine;

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