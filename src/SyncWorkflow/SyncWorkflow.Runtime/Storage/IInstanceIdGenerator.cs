namespace SyncWorkflow.Storage;

public interface IInstanceIdGenerator<in TElement>
{
    Guid GenerateId(TElement element);
}