using Anch.Core.DictionaryCache;
using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence;

public class CachedWorkflowRepositoryFactory(IWorkflowRepositoryFactory repositoryFactory) : IWorkflowRepositoryFactory
{
    private readonly IDictionaryCache<WorkflowDefinitionIdentity, IWorkflowRepository> cache =
        new DictionaryCache<WorkflowDefinitionIdentity, IWorkflowRepository>(repositoryFactory.Create);

    public IWorkflowRepository Create(WorkflowDefinitionIdentity workflowDefinitionIdentity) => this.cache[workflowDefinitionIdentity];
}