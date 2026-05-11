using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Persistence;

public class WorkflowRootRepository(
    IWorkflowSource workflowSource,
    [FromKeyedServices(IWorkflowRepositoryFactory.CacheKey)] IWorkflowRepositoryFactory repositoryFactory) : IWorkflowRepository
{
    public ValueTask SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken cancellationToken) =>
        repositoryFactory.Create(workflowInstance.Definition.Identity).SaveWorkflowInstance(workflowInstance, cancellationToken);

    public ValueTask<WorkflowInstance?> TryGetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken cancellationToken) =>

        this.GetActualRepositories(identity.Definition)
            .Select((rep, ct) => rep.TryGetWorkflowInstance(identity, ct))
            .FirstOrDefaultAsync(wfInstance => wfInstance != null, cancellationToken);

    public ValueTask<StateInstance?> TryGetStateInstance(StateInstanceIdentity identity, CancellationToken cancellationToken) =>

        this.GetActualRepositories(identity.Definition)
            .Select((rep, ct) => rep.TryGetStateInstance(identity, ct))
            .FirstOrDefaultAsync(stateInstance => stateInstance != null, cancellationToken);

    public IAsyncEnumerable<WorkflowInstance> GetWorkflowInstances() =>

        this.GetActualRepositories(null).SelectMany(rep => rep.GetWorkflowInstances());

    public IAsyncEnumerable<WaitEventInfo> GetWaitEvents() =>

        this.GetActualRepositories(null).SelectMany(rep => rep.GetWaitEvents());

    public IAsyncEnumerable<WaitEventInfo> GetWaitEvents(PushEventInfo pushEventInfo) =>

        this.GetActualRepositories(pushEventInfo.TargetState?.Identity.Definition).SelectMany(rep => rep.GetWaitEvents(pushEventInfo));

    private IAsyncEnumerable<IWorkflowRepository> GetActualRepositories(WorkflowDefinitionIdentity? workflowDefinitionIdentity) =>

        this.GetActualWorkflowDefinitions(workflowDefinitionIdentity).ToAsyncEnumerable().Select(repositoryFactory.Create);

    private IEnumerable<WorkflowDefinitionIdentity> GetActualWorkflowDefinitions(WorkflowDefinitionIdentity? workflowDefinitionIdentity) =>

        workflowDefinitionIdentity == null ? workflowSource.Workflows.Keys : new[] { workflowDefinitionIdentity };
}