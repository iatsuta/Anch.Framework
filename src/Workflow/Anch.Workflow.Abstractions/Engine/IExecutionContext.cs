using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Engine;

public interface IExecutionContext
{
    StateInstance StateInstance { get; }

    CancellationToken CancellationToken { get; }

    WaitEventInfo? CallbackEventInfo { get; }

    object Source { get; }

    WorkflowInstance WorkflowInstance { get; }
}