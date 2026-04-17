namespace SyncWorkflow.Storage.Inline;

public interface IStateInstanceSerializerFactory
{
    IStateInstanceSerializer<TSource> Create<TSource>(IWorkflow<TSource> workflow);
}