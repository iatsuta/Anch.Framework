using CommonFramework;
using SyncWorkflow.Domain.Definition;
using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Storage;

public class WorkflowStorage(ISpecificWorkflowStorageSource specificWorkflowStorageSource)
    : IWorkflowStorage
{
    private readonly Lazy<IReadOnlyDictionary<WorkflowDefinitionIdentity, ISpecificWorkflowStorage>> lazySpecificStorageDict = LazyHelper.Create(() => specificWorkflowStorageSource.GetSpecificStorageDict());

    private IReadOnlyDictionary<WorkflowDefinitionIdentity, ISpecificWorkflowStorage> SpecificStorageDict => this.lazySpecificStorageDict.Value;


    public ISpecificWorkflowStorage GetSpecificStorage(WorkflowDefinitionIdentity identity)
    {
        return this.SpecificStorageDict[identity];
    }

    public async Task SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken cancellationToken = default)
    {
        await this.GetSpecificStorage(workflowInstance.Definition.Identity).SaveWorkflowInstance(workflowInstance, cancellationToken);
    }

    public async Task<WorkflowInstance> GetWorkflowInstance(WorkflowInstanceFullIdentity identity, CancellationToken cancellationToken = default)
    {
        return await this.GetSpecificStorage(identity.Workflow).GetWorkflowInstance(identity, cancellationToken);
    }

    public async Task<StateInstance> GetStateInstance(StateInstanceFullIdentity identity, CancellationToken cancellationToken = default)
    {
        return await this.GetSpecificStorage(identity.Workflow).GetStateInstance(identity, cancellationToken);
    }

    public async Task<List<WorkflowInstance>> GetWorkflowInstances(CancellationToken cancellationToken = default)
    {
        var result = await Task.WhenAll(
            this.SpecificStorageDict
                .Values
                .Select(specificStorage => specificStorage.GetWorkflowInstances(cancellationToken)));

        return result.SelectMany().ToList();
    }

    public async Task<List<WaitEventInfo>> GetWaitEvents(CancellationToken cancellationToken = default)
    {
        var result = await Task.WhenAll(
            this.SpecificStorageDict
                .Values
                .Select(specificStorage => specificStorage.GetWaitEvents(cancellationToken)));

        return result.SelectMany().ToList();
    }

    public async Task<List<WaitEventInfo>> GetWaitEvents(PushEventInfo pushEventInfo, CancellationToken cancellationToken = default)
    {
        if (pushEventInfo.TargetState != null)
        {
            return await this.GetSpecificStorage(pushEventInfo.TargetState.Identity.Workflow).GetWaitEvents(pushEventInfo, cancellationToken);
        }
        else
        {
            var allResult = await Task.WhenAll(this.SpecificStorageDict.Values.Select(specificStorage => specificStorage.GetWaitEvents(pushEventInfo, cancellationToken)));

            return allResult.SelectMany().ToList();
        }
    }

    public async Task FlushChanges(CancellationToken cancellationToken = default)
    {
        foreach (var specificStorage in this.SpecificStorageDict.Values)
        {
            await specificStorage.FlushChanges(cancellationToken);
        }
    }
}