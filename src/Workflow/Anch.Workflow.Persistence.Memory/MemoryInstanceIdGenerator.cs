namespace Anch.Workflow.Persistence.Memory;

public class MemoryInstanceIdGenerator<TElement> : IInstanceIdGenerator<TElement>
{
    public Guid GenerateId(TElement _)
    {
        return Guid.NewGuid();
    }
}