using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Storage.Inline.IdGenerator;

public class InlineIdGenerator<TElement>(Func<TElement, WorkflowInstance> pathToWi) : IInstanceIdGenerator<TElement>
{
    public Guid GenerateId(TElement element)
    {
        var wi = pathToWi(element);

        throw new NotImplementedException();

        //var source = (IIdentityObject)wi.Source;

        //return source.Id;
    }
}