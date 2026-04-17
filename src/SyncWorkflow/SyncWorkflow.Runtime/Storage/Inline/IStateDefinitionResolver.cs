using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Storage.Inline;

public interface IStateDefinitionResolver<in TSource>
{
    IStateDefinition GetCurrentStateDefinition(TSource source);
}