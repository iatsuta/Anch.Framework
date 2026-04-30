using Anch.Workflow.Definition;

namespace Anch.Workflow.Storage.Inline;

public interface IStateDefinitionResolver<in TSource>
{
    IStateDefinition GetCurrentStateDefinition(TSource source);
}