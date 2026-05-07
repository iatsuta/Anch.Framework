using Anch.GenericQueryable;
using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Persistence.Inline;

public class InlineWorkflowRepository<TSource, TStatus>(
    IInstanceIdGenerator<WorkflowInstance> workflowInstanceIdGenerator,
    IInstanceIdGenerator<StateInstance> stateInstanceIdGenerator,
    IWorkflowInstanceSerializerFactory workflowInstanceSerializerFactory,
    IWorkflowDefinition<TSource, TStatus> workflowDefinition,
    IInlineStorage<TSource> persistSource)
    : IWorkflowRepository

    where TSource : class
    where TStatus : struct
{
    private readonly IWorkflowInstanceSerializer<TSource> workflowInstanceSerializer = workflowInstanceSerializerFactory.Create(workflowDefinition);

    public IWorkflowDefinition WorkflowDefinition { get; } = workflowDefinition;

    public async ValueTask SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken cancellationToken)
    {
        if (workflowInstance.Definition != this.WorkflowDefinition)
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

    public IAsyncEnumerable<WaitEventInfo> GetWaitEvents()
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<WaitEventInfo> GetWaitEvents(PushEventInfo pushEventInfo)
    {
        var definition = pushEventInfo.TargetState?.Definition;

        if (pushEventInfo.TargetState is { } targetState)
        {
            if (targetState.Workflow.Definition == this.WorkflowDefinition)
            {
                return targetState.WaitEvents.ToAsyncEnumerable();
            }
            else
            {
                return AsyncEnumerable.Empty<WaitEventInfo>();
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public async ValueTask<WorkflowInstance?> TryGetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken cancellationToken)
    {
        if (identity.Definition != null && identity.Definition != this.WorkflowDefinition.Identity)
        {
            return null;
        }
        else
        {
            var source = await persistSource
                .GetQueryable()
                .Where(persistSource.GetFilter(identity with { Definition = this.WorkflowDefinition.Identity }))
                .GenericSingleOrDefaultAsync(cancellationToken);

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

    public async ValueTask<StateInstance?> TryGetStateInstance(StateInstanceIdentity identity, CancellationToken cancellationToken)
    {
        if (identity.Definition != null && identity.Definition != this.WorkflowDefinition.Identity)
        {
            return null;
        }
        else
        {
            var source = await persistSource
                .GetQueryable()
                .Where(persistSource.GetFilter(identity with { Definition = this.WorkflowDefinition.Identity }))
                .GenericSingleOrDefaultAsync(cancellationToken);

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
}