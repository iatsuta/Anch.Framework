using System.Linq.Expressions;

using Anch.GenericRepository;
using Anch.IdentitySource;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Persistence.Inline;

public class InlineStorage<TSource>(IGenericRepository genericRepository, IQueryableSource queryableSource, IIdentityInfo<TSource, Guid> identityInfo)
    : IInlineStorage<TSource>
    where TSource : class
{
    public async ValueTask Save(TSource source, CancellationToken ct) => await genericRepository.SaveAsync(source, ct);

    public ValueTask FlushChanges(CancellationToken ct) => ValueTask.CompletedTask;

    public IQueryable<TSource> GetQueryable() => queryableSource.GetQueryable<TSource>();

    public Expression<Func<TSource, bool>> GetFilter(WorkflowInstanceIdentity wi) => identityInfo.CreateFilter(wi.Id);

    public Expression<Func<TSource, bool>> GetFilter(StateInstanceIdentity si) => identityInfo.CreateFilter(si.Id);
}