using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Serialization.Inline.IdGenerator;

public class InlineIdGenerator<TElement>(Func<TElement, WorkflowInstance> _) : IInstanceIdGenerator<TElement>
{
    public Func<TElement, WorkflowInstance> Unknown { get; } = _;

    public Guid GenerateId(TElement element)
    {
        throw new NotImplementedException();
        //var wi = pathToWi(element);

        //return (Guid)getIdDelegate.DynamicInvoke(wi.Source)!;
    }
}