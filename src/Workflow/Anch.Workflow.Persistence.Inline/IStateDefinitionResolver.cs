using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Persistence.Inline;

public interface IStateDefinitionResolver<TSource, TStatus>
    where TSource : class
    where TStatus : struct
{
    IStateDefinition<TSource, TStatus> GetCurrentStateDefinition(TSource source);
}