using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence.Inline;

public class StateDefinitionResolverFactory : IStateDefinitionResolverFactory
{
    public IStateDefinitionResolver<TSource, TStatus> Create<TSource, TStatus>(IWorkflowDefinition<TSource, TStatus> workflowDefinition)
        where TSource : class
        where TStatus : struct
    {
        return new StateDefinitionResolver<TSource, TStatus>(workflowDefinition);
    }
}