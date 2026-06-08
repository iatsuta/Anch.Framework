using System.Linq.Expressions;

using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Serialization.Inline;

public interface IInlineStorage<TSource>
{
    ValueTask Save(TSource source, CancellationToken ct);

    ValueTask FlushChanges(CancellationToken ct);

    IQueryable<TSource> GetQueryable();

    Expression<Func<TSource, bool>> GetFilter(WorkflowInstanceIdentity wi);

    Expression<Func<TSource, bool>> GetFilter(StateInstanceIdentity si);
}