using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Execution;

namespace Anch.Workflow;

public interface IWorkflowMachine
{
    WorkflowInstance WorkflowInstance { get; }

    void SetStartState();

    ValueTask Save(CancellationToken ct);

    ValueTask<WorkflowProcessResult> ProcessWorkflow(CancellationToken ct);

    ValueTask<WorkflowProcessResult> ProcessWorkflow(ExecutionResult executionResult, CancellationToken ct);

    ValueTask<WorkflowProcessResult> PushReleasedEvent(WaitEventInfo releasedEventInfo, CancellationToken ct);

    ValueTask<WorkflowProcessResult> Terminate(CancellationToken ct);
}