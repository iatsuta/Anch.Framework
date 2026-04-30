using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Storage.Inline;

public class InlineSpecificWorkflowExternalStorage<TSource>(
    IWorkflow<TSource> workflow,
    IInstanceIdGenerator<WorkflowInstance> workflowInstanceIdGenerator,
    IInstanceIdGenerator<StateInstance> stateInstanceIdGenerator,
    IWorkflowInstanceSerializerFactory workflowInstanceSerializerFactory,
    IInlineStorage<TSource> persistSource)
    : ISpecificWorkflowExternalStorage
{
    private readonly IWorkflowInstanceSerializer<TSource> workflowInstanceSerializer = workflowInstanceSerializerFactory.Create(workflow);

    public IWorkflow Workflow { get; } = workflow;

    public async Task SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken cancellationToken = default)
    {
        if (workflowInstance.Definition != this.Workflow.Definition)
        {
            throw new InvalidOperationException("Wrong storage");
        }

        if (workflowInstance.Id == Guid.Empty)
        {
            workflowInstance.Id = workflowInstanceIdGenerator.GenerateId(workflowInstance);
        }

        var currentState = workflowInstance.CurrentState;

        if (currentState.Id == Guid.Empty)
        {
            currentState.Id = stateInstanceIdGenerator.GenerateId(currentState);
        }

        this.workflowInstanceSerializer.Serialize(workflowInstance);

        await persistSource.Save((TSource)workflowInstance.Source, cancellationToken);
    }

    public async Task<List<WaitEventInfo>> GetWaitEvents(CancellationToken cancellationToken = default)
    {
        return [];

        //throw new NotImplementedException();
    }

    public async Task<List<WaitEventInfo>> GetWaitEvents(PushEventInfo pushEventInfo, CancellationToken cancellationToken = default)
    {
        return [];
        //throw new NotImplementedException();
    }

    public async Task<WorkflowInstance> GetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken cancellationToken = default)
    {
        var queryable = persistSource.GetQueryable(cancellationToken);

        var source = queryable.Where(persistSource.GetFilter(identity.ToFull(this.Workflow.Definition.Identity))).Single();

        var wi = this.workflowInstanceSerializer.Deserialize(source);

#if DEBUG
        if (identity != wi.Identity)
        {
            throw new InvalidOperationException();
        }
#endif

        return wi;
    }

    public async Task<StateInstance> GetStateInstance(StateInstanceIdentity identity, CancellationToken cancellationToken = default)
    {
        var queryable = persistSource.GetQueryable(cancellationToken);

        var source = queryable.Where(persistSource.GetFilter(identity.ToFull(this.Workflow.Definition.Identity))).Single();

        var wi = this.workflowInstanceSerializer.Deserialize(source);

#if DEBUG
        if (identity != wi.CurrentState.Identity)
        {
            throw new InvalidOperationException();
        }
#endif

        return wi.CurrentState;
    }

    public Task<List<WorkflowInstance>> GetWorkflowInstances(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task FlushChanges(CancellationToken cancellationToken = default)
    {
        await persistSource.FlushChanges(cancellationToken);
    }
}