using Anch.GenericQueryable;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Serialization.Inline;

public class InlineWorkflowRepository<TSource>(
    IWorkflowDefinition workflowDefinition,
    IInstanceIdGenerator<WorkflowInstance> workflowInstanceIdGenerator,
    IInstanceIdGenerator<StateInstance> stateInstanceIdGenerator,
    IWorkflowInstanceSerializerFactory workflowInstanceSerializerFactory,
    IInlineStorage<TSource> persistSource)
    : IWorkflowRepository

    where TSource : notnull
{
    private readonly IWorkflowInstanceSerializer workflowInstanceSerializer = workflowInstanceSerializerFactory.Create(workflowDefinition);

    public IWorkflowDefinition WorkflowDefinitionBuilder { get; } = workflowDefinition;

    public async ValueTask SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken ct)
    {
        if (workflowInstance.Definition != this.WorkflowDefinitionBuilder)
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

        await persistSource.Save((TSource)workflowInstance.Source, ct);
    }

    public IAsyncEnumerable<WaitEventInfo> GetWaitEvents()
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<WaitEventInfo> GetWaitEvents(PushEventInfo pushEventInfo)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<WorkflowInstance?> TryGetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken ct)
    {
        if (identity.Definition != null && identity.Definition != this.WorkflowDefinitionBuilder.Identity)
        {
            return null;
        }
        else
        {
            var source = await persistSource
                .GetQueryable()
                .Where(persistSource.GetFilter(identity with { Definition = this.WorkflowDefinitionBuilder.Identity }))
                .GenericSingleOrDefaultAsync(ct);

            if (source is not null)
            {
                var wi = this.workflowInstanceSerializer.Deserialize(source);

#if DEBUG
                if (identity != wi.Identity)
                {
                    throw new InvalidOperationException();
                }
#endif

                return wi;
            }
            else
            {
                return null;
            }
        }
    }

    public async ValueTask<StateInstance?> TryGetStateInstance(StateInstanceIdentity identity, CancellationToken ct)
    {
        if (identity.Definition != null && identity.Definition != this.WorkflowDefinitionBuilder.Identity)
        {
            return null;
        }
        else
        {
            var source = await persistSource
                .GetQueryable()
                .Where(persistSource.GetFilter(identity with { Definition = this.WorkflowDefinitionBuilder.Identity }))
                .GenericSingleOrDefaultAsync(ct);

            if (source is not null)
            {
                var wi = this.workflowInstanceSerializer.Deserialize(source);

#if DEBUG
                if (identity != wi.CurrentState.Identity)
                {
                    throw new InvalidOperationException();
                }
#endif

                return wi.CurrentState;
            }
            else
            {
                return null;
            }
        }
    }

    public IAsyncEnumerable<WorkflowInstance> GetWorkflowInstances()
    {
        var queryable = persistSource.GetQueryable();

        return queryable
            .GenericAsAsyncEnumerable()
            .Select(source => this.workflowInstanceSerializer.Deserialize(source!));
    }

    public async ValueTask FlushChanges(CancellationToken ct)
    {
        await persistSource.FlushChanges(ct);
    }
}