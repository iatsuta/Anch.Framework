namespace Anch.Workflow.Storage;

public class RandomIdGenerator<TElement> : IInstanceIdGenerator<TElement>
{
    public Guid GenerateId(TElement _)
    {
        return Guid.NewGuid();
    }
}