using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Engine;

public interface IWorkflowMachine
{
    WorkflowInstance WorkflowInstance { get; }

    void SetStartState();

    Task Save(CancellationToken cancellationToken = default);

    Task<WorkflowProcessResult> ProcessWorkflow(CancellationToken cancellationToken = default);

    Task<WorkflowProcessResult> PushReleasedEvent(WaitEventInfo releasedEventInfo, CancellationToken cancellationToken = default);

    Task<WorkflowProcessResult> Terminate(CancellationToken cancellationToken = default);
}