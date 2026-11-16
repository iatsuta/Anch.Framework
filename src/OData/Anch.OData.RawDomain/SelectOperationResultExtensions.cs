namespace Anch.OData.Domain;

/// <summary>Extensions for <see cref="SelectOperationResult{T}"/>.</summary>
public static class SelectOperationResultExtensions
{
    /// <summary>Wraps items into a result.</summary>
    /// <param name="items">The items of the page.</param>
    /// <param name="totalCount">The number of matching items without paging.</param>
    public static SelectOperationResult<T> ToSelectOperationResult<T>(this IEnumerable<T> items, int totalCount)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));

        return new SelectOperationResult<T>(items, totalCount);
    }

    /// <summary>Projects the items; the total count is kept.</summary>
    /// <param name="source">The result.</param>
    /// <param name="selector">Converts an item.</param>
    public static SelectOperationResult<TResult> Select<TSource, TResult>(this SelectOperationResult<TSource> source, Func<TSource, TResult> selector)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return source.Items.Select(selector).ToSelectOperationResult(source.TotalCount);
    }

    /// <summary>Filters the items of the page; the total count is kept.</summary>
    /// <param name="source">The result.</param>
    /// <param name="filter">The condition.</param>
    public static SelectOperationResult<T> Where<T>(this SelectOperationResult<T> source, Func<T, bool> filter)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (filter is null) throw new ArgumentNullException(nameof(filter));

        return source.Items.Where(filter).ToSelectOperationResult(source.TotalCount);
    }
}
