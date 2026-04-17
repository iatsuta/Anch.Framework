using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Storage;

public class MemorySpecificWorkflowExternalStorage(
    IWorkflow workflow,
    IInstanceIdGenerator<WorkflowInstance> workflowInstanceIdGenerator,
    IInstanceIdGenerator<StateInstance> stateInstanceIdGenerator)
    : ISpecificWorkflowExternalStorage
{
    private readonly Dictionary<WorkflowInstanceIdentity, WorkflowInstance> Values = new();

    public IWorkflow Workflow { get; } = workflow;

    public async Task SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken cancellationToken = default)
    {
        if (workflowInstance.Definition != this.Workflow.Definition)
        {
            throw new InvalidOperationException("Wrong storage");
        }

        if (workflowInstance.Id.IsDefault())
        {
            workflowInstance.Id = workflowInstanceIdGenerator.GenerateId(workflowInstance);
        }

        var currentState = workflowInstance.CurrentState;

        if (currentState.Id.IsDefault())
        {
            currentState.Id = stateInstanceIdGenerator.GenerateId(currentState);
        }

        this.Values[workflowInstance.Identity] = workflowInstance;
    }

    public Task<List<WaitEventInfo>> ReleaseWaitEvents(PushEventInfo pushEventInfo, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<WorkflowInstance> GetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken cancellationToken = default)
    {
        return this.Values[identity];
    }

    public async Task<StateInstance> GetStateInstance(StateInstanceIdentity identity, CancellationToken cancellationToken = default)
    {
        return this.Values
            .Values
            .Select(wi => wi.CurrentState)
            .Single(si => si.Identity == identity);
    }

    public async Task<List<WorkflowInstance>> GetWorkflowInstances(CancellationToken cancellationToken = default)
    {
        return this.Values.Values.ToList();
    }

    public async Task<List<WaitEventInfo>> GetWaitEvents(CancellationToken cancellationToken = default)
    {
        return this.Values.Values.SelectMany(wi => wi.CurrentState.WaitEvents).ToList();
    }

    public async Task<List<WaitEventInfo>> GetWaitEvents(PushEventInfo pushEventInfo, CancellationToken cancellationToken = default)
    {
        var request =

            from wi in this.Values.Values

            from waitEventInfo in wi.CurrentState.WaitEvents

            where pushEventInfo.IsMatched(waitEventInfo)

            select waitEventInfo;

        return request.ToList();
    }

    public async Task FlushChanges(CancellationToken cancellationToken = default)
    {
    }
}