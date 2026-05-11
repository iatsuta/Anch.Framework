using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Persistence;

public interface IWorkflowRepository
{
    public const string RootKey = "Root";

    ValueTask SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken cancellationToken = default);


    ValueTask<WorkflowInstance?> TryGetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken cancellationToken = default);

    ValueTask<StateInstance?> TryGetStateInstance(StateInstanceIdentity identity, CancellationToken cancellationToken = default);


    IAsyncEnumerable<WaitEventInfo> GetWaitEvents(PushEventInfo pushEventInfo);

    IAsyncEnumerable<WaitEventInfo> GetWaitEvents();

    IAsyncEnumerable<WorkflowInstance> GetWorkflowInstances();
}