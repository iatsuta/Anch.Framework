namespace Anch.OData.Domain;

public static class SelectOperationResultExtensions
{
    public static SelectOperationResult<T> ToSelectOperationResult<T>(this IEnumerable<T> items, int totalCount)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));

        return new SelectOperationResult<T>(items, totalCount);
    }

    public static SelectOperationResult<TResult> Select<TSource, TResult>(this SelectOperationResult<TSource> source, Func<TSource, TResult> selector)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return source.Items.Select(selector).ToSelectOperationResult(source.TotalCount);
    }

    public static SelectOperationResult<T> Where<T>(this SelectOperationResult<T> source, Func<T, bool> filter)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (filter is null) throw new ArgumentNullException(nameof(filter));

        return source.Items.Where(filter).ToSelectOperationResult(source.TotalCount);
    }
}
