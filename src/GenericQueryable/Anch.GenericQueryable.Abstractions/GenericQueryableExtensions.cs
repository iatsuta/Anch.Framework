using System.Linq.Expressions;

using Anch.GenericQueryable.Services;

namespace Anch.GenericQueryable;

public static class GenericQueryableExtensions
{
    extension<TSource>(IQueryable<TSource> source)
    {
        public Task<TSource[]> GenericToArrayAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericToArrayAsync(ct));

        public Task<List<TSource>> GenericToListAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericToListAsync(ct));

        public Task<HashSet<TSource>> GenericToHashSetAsync(CancellationToken ct) =>
            source.GenericToHashSetAsync(null, ct);

        public Task<HashSet<TSource>> GenericToHashSetAsync(IEqualityComparer<TSource>? comparer,
            CancellationToken ct) =>
            source.Execute(() => source.GenericToHashSetAsync(comparer, ct));

        public Task<Dictionary<TKey, TSource>> GenericToDictionaryAsync<TKey>(Func<TSource, TKey> keySelector,
            CancellationToken ct)
            where TKey : notnull =>
            source.GenericToDictionaryAsync(keySelector, null, ct);

        public Task<Dictionary<TKey, TSource>> GenericToDictionaryAsync<TKey>(Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey>? comparer,
            CancellationToken ct)
            where TKey : notnull =>
            source.GenericToDictionaryAsync(keySelector, v => v, comparer, ct);

        public Task<Dictionary<TKey, TElement>> GenericToDictionaryAsync<TKey, TElement>(Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            CancellationToken ct)
            where TKey : notnull =>
            source.GenericToDictionaryAsync(keySelector, elementSelector, null, ct);

        public Task<Dictionary<TKey, TElement>> GenericToDictionaryAsync<TKey, TElement>(Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey>? comparer,
            CancellationToken ct)
            where TKey : notnull =>
            source.Execute(() => source.GenericToDictionaryAsync(keySelector, elementSelector, comparer, ct));

        public Task<TSource> GenericSingleAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericSingleAsync(ct));

        public Task<TSource> GenericSingleAsync(Expression<Func<TSource, bool>> filter,
            CancellationToken ct) =>
            source.Execute(() => source.GenericSingleAsync(filter, ct));

        public Task<TSource?> GenericSingleOrDefaultAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericSingleOrDefaultAsync(ct));

        public Task<TSource?> GenericSingleOrDefaultAsync(Expression<Func<TSource, bool>> filter,
            CancellationToken ct) =>
            source.Execute(() => source.GenericSingleOrDefaultAsync(filter, ct));

        public Task<TSource> GenericFirstAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericFirstAsync(ct));

        public Task<TSource?> GenericFirstOrDefaultAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericFirstOrDefaultAsync(ct));

        public Task<int> GenericCountAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericCountAsync(ct));

        public Task<bool> GenericAllAsync(Expression<Func<TSource, bool>> filter,
            CancellationToken ct) =>
            source.Execute(() => source.GenericAllAsync(filter, ct));

        public Task<bool> GenericAnyAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericAnyAsync(ct));

        public Task<bool> GenericAnyAsync(Expression<Func<TSource, bool>> filter,
            CancellationToken ct) =>
            source.Execute(() => source.GenericAnyAsync(filter, ct));

        public Task<TSource?> GenericFirstOrDefaultAsync(Expression<Func<TSource, bool>> filter,
            CancellationToken ct) =>
            source.Execute(() => source.GenericFirstOrDefaultAsync(filter, ct));

        public Task<bool> GenericContainsAsync(TSource value, CancellationToken ct) =>
            source.Execute(() => source.GenericContainsAsync(value, ct));

        public Task<decimal?> GenericSumAsync(Expression<Func<TSource, decimal?>> selector,
            CancellationToken ct) =>
            source.Execute(() => source.GenericSumAsync(selector, ct));

        public IAsyncEnumerable<TSource> GenericAsAsyncEnumerable() => source.Execute(() => source.GenericAsAsyncEnumerable());

        public TResult Execute<TResult>(Expression<Func<TResult>> callExpression) =>
            source.Execute(executor => executor.Execute(callExpression));

        public TResult Execute<TResult>(Func<IGenericQueryableExecutor, TResult> execute) =>
            execute((source.Provider as IGenericQueryProvider)?.Executor ?? GenericQueryableExecutor.Sync);
    }

    extension(IQueryable<decimal?> source)
    {
        public Task<decimal?> GenericSumAsync(
            CancellationToken ct) =>
            source.Execute(() => source.GenericSumAsync(ct));
    }
}