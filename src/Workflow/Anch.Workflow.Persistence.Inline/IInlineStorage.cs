using System.Linq.Expressions;

using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Persistence.Inline;

public interface IInlineStorage<TSource>
    where TSource : class
{
    ValueTask Save(TSource source, CancellationToken cancellationToken);

    IQueryable<TSource> GetQueryable();

    Expression<Func<TSource, bool>> GetFilter(WorkflowInstanceIdentity wi);

    Expression<Func<TSource, bool>> GetFilter(StateInstanceIdentity si);
}