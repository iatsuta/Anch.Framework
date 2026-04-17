using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Storage;

public interface IWorkflowStorageBase<in TResolveWorkflowInstanceIdentity, in TResolveStateInstanceIdentity>
{
    Task SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken cancellationToken = default);

    Task<List<WaitEventInfo>> GetWaitEvents(PushEventInfo pushEventInfo, CancellationToken cancellationToken = default);


    Task FlushChanges(CancellationToken cancellationToken = default);


    Task<WorkflowInstance> GetWorkflowInstance(TResolveWorkflowInstanceIdentity identity, CancellationToken cancellationToken = default);

    Task<StateInstance> GetStateInstance(TResolveStateInstanceIdentity identity, CancellationToken cancellationToken = default);


    Task<List<WorkflowInstance>> GetWorkflowInstances(CancellationToken cancellationToken = default);

    Task<List<WaitEventInfo>> GetWaitEvents(CancellationToken cancellationToken = default);
}