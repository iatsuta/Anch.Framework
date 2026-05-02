using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow;

public interface IWorkflowMachine
{
    WorkflowInstance WorkflowInstance { get; }

    void SetStartState();

    Task Save(CancellationToken cancellationToken = default);

    Task<WorkflowProcessResult> ProcessWorkflow(CancellationToken cancellationToken = default);

    Task<WorkflowProcessResult> PushReleasedEvent(WaitEventInfo releasedEventInfo, CancellationToken cancellationToken = default);

    Task<WorkflowProcessResult> Terminate(CancellationToken cancellationToken = default);
}