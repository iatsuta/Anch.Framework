using System.Linq.Expressions;

using Anch.GenericQueryable.Services;

namespace Anch.GenericQueryable;

/// <summary>
/// ORM-independent async LINQ operators for <see cref="IQueryable{T}"/>.
/// </summary>
/// <remarks>
/// Application code calls these methods instead of the ORM-specific ones (<c>ToListAsync</c>, <c>FirstOrDefaultAsync</c>, ...),
/// so it does not depend on Entity Framework or NHibernate. Each call is executed by the <see cref="IGenericQueryableExecutor"/>
/// taken from the queryable's provider (<see cref="IGenericQueryProvider"/>), which redirects it to the real ORM method.
/// A queryable whose provider is not an <see cref="IGenericQueryProvider"/> (for example LINQ to Objects) is executed synchronously.
/// </remarks>
public static class GenericQueryableExtensions
{
    extension<TSource>(IQueryable<TSource> source)
    {
        /// <summary>Asynchronously materializes the query into an array.</summary>
        /// <param name="ct">Cancellation token.</param>
        public Task<TSource[]> GenericToArrayAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericToArrayAsync(ct));

        /// <summary>Asynchronously materializes the query into a list.</summary>
        /// <param name="ct">Cancellation token.</param>
        public Task<List<TSource>> GenericToListAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericToListAsync(ct));

        /// <summary>Asynchronously materializes the query into a hash set using the default comparer.</summary>
        /// <param name="ct">Cancellation token.</param>
        public Task<HashSet<TSource>> GenericToHashSetAsync(CancellationToken ct) =>
            source.GenericToHashSetAsync(null, ct);

        /// <summary>Asynchronously materializes the query into a hash set.</summary>
        /// <param name="comparer">Comparer for the set; <c>null</c> for the default one.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<HashSet<TSource>> GenericToHashSetAsync(IEqualityComparer<TSource>? comparer,
            CancellationToken ct) =>
            source.Execute(() => source.GenericToHashSetAsync(comparer, ct));

        /// <summary>Asynchronously materializes the query into a dictionary keyed by <paramref name="keySelector"/>; the items themselves are the values.</summary>
        /// <param name="keySelector">Selects the dictionary key of an item.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<Dictionary<TKey, TSource>> GenericToDictionaryAsync<TKey>(Func<TSource, TKey> keySelector,
            CancellationToken ct)
            where TKey : notnull =>
            source.GenericToDictionaryAsync(keySelector, null, ct);

        /// <summary>Asynchronously materializes the query into a dictionary keyed by <paramref name="keySelector"/>; the items themselves are the values.</summary>
        /// <param name="keySelector">Selects the dictionary key of an item.</param>
        /// <param name="comparer">Key comparer; <c>null</c> for the default one.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<Dictionary<TKey, TSource>> GenericToDictionaryAsync<TKey>(Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey>? comparer,
            CancellationToken ct)
            where TKey : notnull =>
            source.GenericToDictionaryAsync(keySelector, v => v, comparer, ct);

        /// <summary>Asynchronously materializes the query into a dictionary with projected values.</summary>
        /// <param name="keySelector">Selects the dictionary key of an item.</param>
        /// <param name="elementSelector">Selects the dictionary value of an item.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<Dictionary<TKey, TElement>> GenericToDictionaryAsync<TKey, TElement>(Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            CancellationToken ct)
            where TKey : notnull =>
            source.GenericToDictionaryAsync(keySelector, elementSelector, null, ct);

        /// <summary>Asynchronously materializes the query into a dictionary with projected values.</summary>
        /// <param name="keySelector">Selects the dictionary key of an item.</param>
        /// <param name="elementSelector">Selects the dictionary value of an item.</param>
        /// <param name="comparer">Key comparer; <c>null</c> for the default one.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<Dictionary<TKey, TElement>> GenericToDictionaryAsync<TKey, TElement>(Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey>? comparer,
            CancellationToken ct)
            where TKey : notnull =>
            source.Execute(() => source.GenericToDictionaryAsync(keySelector, elementSelector, comparer, ct));

        /// <summary>Returns the only element of the query; throws if there are zero or several.</summary>
        /// <param name="ct">Cancellation token.</param>
        public Task<TSource> GenericSingleAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericSingleAsync(ct));

        /// <summary>Returns the only element that satisfies <paramref name="filter"/>; throws if there are zero or several.</summary>
        /// <param name="filter">Condition the element must satisfy.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<TSource> GenericSingleAsync(Expression<Func<TSource, bool>> filter,
            CancellationToken ct) =>
            source.Execute(() => source.GenericSingleAsync(filter, ct));

        /// <summary>Returns the only element of the query, or <c>default</c> if it is empty; throws if there are several.</summary>
        /// <param name="ct">Cancellation token.</param>
        public Task<TSource?> GenericSingleOrDefaultAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericSingleOrDefaultAsync(ct));

        /// <summary>Returns the only element that satisfies <paramref name="filter"/>, or <c>default</c> if there is none; throws if there are several.</summary>
        /// <param name="filter">Condition the element must satisfy.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<TSource?> GenericSingleOrDefaultAsync(Expression<Func<TSource, bool>> filter,
            CancellationToken ct) =>
            source.Execute(() => source.GenericSingleOrDefaultAsync(filter, ct));

        /// <summary>Returns the first element of the query; throws if it is empty.</summary>
        /// <param name="ct">Cancellation token.</param>
        public Task<TSource> GenericFirstAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericFirstAsync(ct));

        /// <summary>Returns the first element of the query, or <c>default</c> if it is empty.</summary>
        /// <param name="ct">Cancellation token.</param>
        public Task<TSource?> GenericFirstOrDefaultAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericFirstOrDefaultAsync(ct));

        /// <summary>Returns the number of elements in the query.</summary>
        /// <param name="ct">Cancellation token.</param>
        public Task<int> GenericCountAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericCountAsync(ct));

        /// <summary>Determines whether every element satisfies <paramref name="filter"/>.</summary>
        /// <param name="filter">Condition to test.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<bool> GenericAllAsync(Expression<Func<TSource, bool>> filter,
            CancellationToken ct) =>
            source.Execute(() => source.GenericAllAsync(filter, ct));

        /// <summary>Determines whether the query contains any element.</summary>
        /// <param name="ct">Cancellation token.</param>
        public Task<bool> GenericAnyAsync(CancellationToken ct) =>
            source.Execute(() => source.GenericAnyAsync(ct));

        /// <summary>Determines whether any element satisfies <paramref name="filter"/>.</summary>
        /// <param name="filter">Condition to test.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<bool> GenericAnyAsync(Expression<Func<TSource, bool>> filter,
            CancellationToken ct) =>
            source.Execute(() => source.GenericAnyAsync(filter, ct));

        /// <summary>Returns the first element that satisfies <paramref name="filter"/>, or <c>default</c> if there is none.</summary>
        /// <param name="filter">Condition the element must satisfy.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<TSource?> GenericFirstOrDefaultAsync(Expression<Func<TSource, bool>> filter,
            CancellationToken ct) =>
            source.Execute(() => source.GenericFirstOrDefaultAsync(filter, ct));

        /// <summary>Determines whether the query contains <paramref name="value"/>.</summary>
        /// <param name="value">The value to look for.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<bool> GenericContainsAsync(TSource value, CancellationToken ct) =>
            source.Execute(() => source.GenericContainsAsync(value, ct));

        /// <summary>Computes the sum of the values projected by <paramref name="selector"/>.</summary>
        /// <param name="selector">Selects the value to sum.</param>
        /// <param name="ct">Cancellation token.</param>
        public Task<decimal?> GenericSumAsync(Expression<Func<TSource, decimal?>> selector,
            CancellationToken ct) =>
            source.Execute(() => source.GenericSumAsync(selector, ct));

        /// <summary>Exposes the query as an <see cref="IAsyncEnumerable{T}"/> executed by the underlying ORM.</summary>
        public IAsyncEnumerable<TSource> GenericAsAsyncEnumerable() => source.Execute(() => source.GenericAsAsyncEnumerable());

        /// <summary>Executes a call expression through the executor of the queryable's provider.</summary>
        /// <param name="callExpression">The call to execute (a <c>GenericXxxAsync</c> call or its ORM equivalent).</param>
        /// <typeparam name="TResult">Result type of the call.</typeparam>
        public TResult Execute<TResult>(Expression<Func<TResult>> callExpression) =>
            source.Execute(executor => executor.Execute(callExpression));

        /// <summary>
        /// Passes the executor of the queryable's provider to <paramref name="execute"/>.
        /// Falls back to <see cref="GenericQueryableExecutor.Sync"/> when the provider is not an <see cref="IGenericQueryProvider"/>.
        /// </summary>
        /// <param name="execute">Operation that uses the executor.</param>
        /// <typeparam name="TResult">Result type of the operation.</typeparam>
        public TResult Execute<TResult>(Func<IGenericQueryableExecutor, TResult> execute) =>
            execute((source.Provider as IGenericQueryProvider)?.Executor ?? GenericQueryableExecutor.Sync);
    }

    extension(IQueryable<decimal?> source)
    {
        /// <summary>Computes the sum of the values of the query.</summary>
        /// <param name="ct">Cancellation token.</param>
        public Task<decimal?> GenericSumAsync(
            CancellationToken ct) =>
            source.Execute(() => source.GenericSumAsync(ct));
    }
}