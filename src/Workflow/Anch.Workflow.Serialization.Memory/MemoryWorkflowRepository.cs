using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Serialization.Memory;

public class MemoryWorkflowRepository(
    MemoryWorkflowRootState rootState,
    IInstanceIdGenerator<WorkflowInstance> workflowInstanceIdGenerator,
    IInstanceIdGenerator<StateInstance> stateInstanceIdGenerator,
    WorkflowDefinitionIdentity workflowDefinitionIdentity) : IWorkflowRepository
{
    public ValueTask SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken cancellationToken)
    {
        if (workflowInstance.Definition.Identity != workflowDefinitionIdentity)
        {
            throw new InvalidOperationException("Wrong repository storage");
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

        rootState.StateInstances[currentState.Identity] = currentState;

        rootState.WorkflowInstances[workflowInstance.Identity] = workflowInstance;

        return ValueTask.CompletedTask;
    }

    public async ValueTask<WorkflowInstance?> TryGetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken cancellationToken)
    {
        if (identity.Definition != null && identity.Definition != workflowDefinitionIdentity)
        {
            return null;
        }
        else
        {
            var actualIdentity = identity with { Definition = workflowDefinitionIdentity };

            return rootState.WorkflowInstances.GetValueOrDefault(actualIdentity);
        }
    }

    public async ValueTask<StateInstance?> TryGetStateInstance(StateInstanceIdentity identity, CancellationToken cancellationToken)
    {
        if (identity.Definition != null && identity.Definition != workflowDefinitionIdentity)
        {
            return null;
        }
        else
        {
            var actualIdentity = identity with { Definition = workflowDefinitionIdentity };

            return rootState.StateInstances.GetValueOrDefault(actualIdentity);
        }
    }

    public IAsyncEnumerable<WaitEventInfo> GetWaitEvents(PushEventInfo pushEventInfo) =>

        this.GetWaitEvents().Where(pushEventInfo.IsMatched);

    public IAsyncEnumerable<WaitEventInfo> GetWaitEvents() =>

        from stateInstance in this.GetStateInstances()

        from waitEventInfo in stateInstance.WaitEvents

        select waitEventInfo;


    public IAsyncEnumerable<WorkflowInstance> GetWorkflowInstances() =>
         rootState.WorkflowInstances.Where(pair => pair.Key.Definition == workflowDefinitionIdentity).Select(pair => pair.Value).ToAsyncEnumerable();

    private IAsyncEnumerable<StateInstance> GetStateInstances() =>
        rootState.StateInstances.Where(pair => pair.Key.Definition == workflowDefinitionIdentity).Select(pair => pair.Value).ToAsyncEnumerable();
}