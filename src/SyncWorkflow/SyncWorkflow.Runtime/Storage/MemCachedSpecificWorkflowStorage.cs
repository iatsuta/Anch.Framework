using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Storage;

public class MemCachedSpecificWorkflowStorage(ISpecificWorkflowExternalStorage externalStorage, IWorkflow workflow)
    : ISpecificWorkflowStorage
{
    private readonly Dictionary<WorkflowInstanceIdentity, WorkflowInstance> cache = new();


    public IWorkflow Workflow { get; } = workflow;

    public virtual async Task SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken cancellationToken = default)
    {
        await externalStorage.SaveWorkflowInstance(workflowInstance, cancellationToken);

        this.cache[workflowInstance.Identity] = workflowInstance;
    }

    public async Task<WorkflowInstance> GetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken cancellationToken = default)
    {
        if (this.cache.TryGetValue(identity, out var wi))
        {
            return wi;
        }
        else
        {
            return this.cache[identity] = await externalStorage.GetWorkflowInstance(identity, cancellationToken);
        }
    }

    public async Task<StateInstance> GetStateInstance(StateInstanceIdentity identity, CancellationToken cancellationToken = default)
    {
        var cachedSi = this.cache.Values.Select(info => info.CurrentState).SingleOrDefault(currentSi => identity == currentSi.Identity);

        if (cachedSi == null)
        {
            var externalSi = await externalStorage.GetStateInstance(identity, cancellationToken);

            this.cache[externalSi.Workflow.Identity] = externalSi.Workflow;

            return externalSi;
        }
        else
        {
            return cachedSi;
        }
    }

    public async Task<List<WorkflowInstance>> GetWorkflowInstances(CancellationToken cancellationToken = default)
    {
        return await externalStorage.GetWorkflowInstances(cancellationToken);
    }

    public async Task<List<WaitEventInfo>> GetWaitEvents(CancellationToken cancellationToken = default)
    {
        return await externalStorage.GetWaitEvents(cancellationToken);
    }

    public async Task FlushChanges(CancellationToken cancellationToken = default)
    {
        await externalStorage.FlushChanges(cancellationToken);
    }

    public async Task<List<WaitEventInfo>> GetWaitEvents(PushEventInfo pushEventInfo, CancellationToken cancellationToken)
    {
        if (pushEventInfo.TargetState != null)
        {
            return pushEventInfo.TargetState.WaitEvents.Where(pushEventInfo.IsMatched).ToList();
        }
        else
        {
            return await externalStorage.GetWaitEvents(pushEventInfo, cancellationToken);
        }
    }
}