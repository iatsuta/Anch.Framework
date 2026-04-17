using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Storage.Inline.IdGenerator;

public class InlineIdGenerator<TElement>(Func<TElement, WorkflowInstance> pathToWi) : IInstanceIdGenerator<TElement>
{
    public Guid GenerateId(TElement element)
    {
        var wi = pathToWi(element);

        var source = (IIdentityObject)wi.Source;

        return source.Id;
    }
}