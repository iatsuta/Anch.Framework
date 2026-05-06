using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;
using Anch.Workflow.Execution;

namespace Anch.Workflow;

public interface IWorkflowMachine
{
    WorkflowInstance WorkflowInstance { get; }

    void SetStartState();

    ValueTask Save(CancellationToken cancellationToken = default);

    ValueTask<WorkflowProcessResult> ProcessWorkflow(CancellationToken cancellationToken = default);

    ValueTask<WorkflowProcessResult> ProcessWorkflow(ExecutionResult executionResult, CancellationToken cancellationToken = default);

    ValueTask<WorkflowProcessResult> PushReleasedEvent(WaitEventInfo releasedEventInfo, CancellationToken cancellationToken = default);

    ValueTask<WorkflowProcessResult> Terminate(CancellationToken cancellationToken = default);
}