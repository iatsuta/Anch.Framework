using Anch.Core;
using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence.Inline;

public class InlineWorkflowRepositoryFactory(IServiceProxyFactory serviceProxyFactory, IWorkflowSource workflowSource) : IWorkflowRepositoryFactory
{
    public IWorkflowRepository Create(WorkflowDefinitionIdentity workflowDefinitionIdentity)
    {
        var workflowDefinition = workflowSource.Workflows[workflowDefinitionIdentity];

        var workflowRepositoryType = typeof(InlineWorkflowRepository<,>).MakeGenericType(workflowDefinition.SourceType, workflowDefinition.StatusType);

        return serviceProxyFactory.Create<IWorkflowRepository>(workflowRepositoryType, workflowDefinition);
    }
}