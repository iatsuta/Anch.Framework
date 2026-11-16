using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence.Inline;

public interface IStateDefinitionResolverFactory
{
    IStateDefinitionResolver<TSource, TStatus> Create<TSource, TStatus>(IWorkflowDefinition<TSource, TStatus> workflowDefinition)
        where TSource : class
        where TStatus : struct;
}