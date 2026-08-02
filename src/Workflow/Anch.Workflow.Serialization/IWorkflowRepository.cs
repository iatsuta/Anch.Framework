using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Serialization;

public interface IWorkflowRepository
{
    public const string RootKey = "Root";

    ValueTask SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken ct);


    ValueTask<WorkflowInstance?> TryGetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken ct);

    ValueTask<StateInstance?> TryGetStateInstance(StateInstanceIdentity identity, CancellationToken ct);


    IAsyncEnumerable<WaitEventInfo> GetWaitEvents(PushEventInfo pushEventInfo);

    IAsyncEnumerable<WaitEventInfo> GetWaitEvents();

    IAsyncEnumerable<WorkflowInstance> GetWorkflowInstances();
}