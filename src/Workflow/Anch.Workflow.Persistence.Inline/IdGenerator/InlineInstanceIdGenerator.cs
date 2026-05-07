using System.Collections.Concurrent;

using Anch.Core;
using Anch.IdentitySource;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Persistence.Inline.IdGenerator;

public class InlineInstanceIdGenerator<TElement>(
    IIdentityInfoSource identityInfoSource,
    Func<TElement, WorkflowInstance> pathToWi) : IInstanceIdGenerator<TElement>
{
    private readonly ConcurrentDictionary<Type, Func<WorkflowInstance, Guid>> funcCache = [];

    public Guid GenerateId(TElement element)
    {
        var wi = pathToWi(element);

        return this.funcCache.GetOrAdd(wi.Definition.SourceType, _ =>
        {
            var identityInfo = identityInfoSource.GetIdentityInfo(wi.Definition.SourceType);

            return new Func<IIdentityInfo<object, Guid>, Func<WorkflowInstance, Guid>>(this.GetFunc<object>)
                .CreateGenericMethod(wi.Definition.SourceType)
                .Invoke<Func<WorkflowInstance, Guid>>(this, identityInfo);
        }).Invoke(wi);
    }

    private Func<WorkflowInstance, Guid> GetFunc<TSource>(IIdentityInfo<TSource, Guid> identityInfo) =>
        wi => identityInfo.Id.Getter((TSource)wi.Source);
}