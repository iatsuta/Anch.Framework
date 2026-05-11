namespace Anch.Workflow.Persistence;

public interface IInstanceIdGenerator<in TElement>
{
    Guid GenerateId(TElement element);
}