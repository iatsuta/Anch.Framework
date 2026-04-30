using System.Linq.Expressions;

using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Storage.Inline;

public interface IInlineStorage<TSource>
{
    Task Save(TSource source, CancellationToken cancellationToken = default);

    Task FlushChanges(CancellationToken cancellationToken = default);

    IQueryable<TSource> GetQueryable(CancellationToken cancellationToken = default);

    Expression<Func<TSource, bool>> GetFilter(WorkflowInstanceFullIdentity wi);

    Expression<Func<TSource, bool>> GetFilter(StateInstanceFullIdentity si);
}