namespace SyncWorkflow.Storage.Inline;

public interface IStateDefinitionResolverFactory
{
    IStateDefinitionResolver<TSource> Create<TSource>(IWorkflow<TSource> workflow);
}