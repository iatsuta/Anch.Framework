using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Persistence;

public static class WorkflowRepositoryExtensions
{
    public static async ValueTask<WorkflowInstance> GetWorkflowInstance(this IWorkflowRepository workflowRepository, WorkflowInstanceIdentity identity,
        CancellationToken cancellationToken = default)
    {
        var result = await workflowRepository.TryGetWorkflowInstance(identity, cancellationToken);

        if (result == null)
        {
            throw new InvalidOperationException($"Workflow instance with identity {identity} not found.");
        }

        return result;
    }

    public static async ValueTask<StateInstance> GetStateInstance(this IWorkflowRepository workflowRepository, StateInstanceIdentity identity,
        CancellationToken cancellationToken = default)
    {
        var result = await workflowRepository.TryGetStateInstance(identity, cancellationToken);

        if (result == null)
        {
            throw new InvalidOperationException($"State instance with identity {identity} not found.");
        }

        return result;
    }
}