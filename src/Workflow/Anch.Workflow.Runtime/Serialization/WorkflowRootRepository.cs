using Anch.Workflow.Domain;
using Anch.Workflow.Domain.Definition;
using Anch.Workflow.Domain.Runtime;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.Serialization;

public class WorkflowRootRepository(
    IWorkflowSource workflowSource,
    [FromKeyedServices(IWorkflowRepositoryFactory.CacheKey)] IWorkflowRepositoryFactory repositoryFactory) : IWorkflowRepository
{
    public ValueTask SaveWorkflowInstance(WorkflowInstance workflowInstance, CancellationToken ct) =>
        repositoryFactory.Create(workflowInstance.Definition.Identity).SaveWorkflowInstance(workflowInstance, ct);

    public ValueTask<WorkflowInstance?> TryGetWorkflowInstance(WorkflowInstanceIdentity identity, CancellationToken ct) =>

        this.GetActualRepositories(identity.Definition)
            .Select((rep, lct) => rep.TryGetWorkflowInstance(identity, lct))
            .FirstOrDefaultAsync(wfInstance => wfInstance != null, ct);

    public ValueTask<StateInstance?> TryGetStateInstance(StateInstanceIdentity identity, CancellationToken ct) =>

        this.GetActualRepositories(identity.Definition)
            .Select((rep, lct) => rep.TryGetStateInstance(identity, lct))
            .FirstOrDefaultAsync(stateInstance => stateInstance != null, ct);

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