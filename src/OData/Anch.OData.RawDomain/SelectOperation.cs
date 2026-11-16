using System.Collections.Immutable;

using Anch.OData.Domain.QueryLanguage;
using Anch.OData.Domain.QueryLanguage.Constant;

namespace Anch.OData.Domain;

/// <summary>An untyped parsed query: a filter, orderings and paging, before binding to a domain type.</summary>
/// <param name="Filter">The <c>$filter</c> lambda.</param>
/// <param name="Orders">The <c>$orderby</c> clauses.</param>
/// <param name="SkipCount">The <c>$skip</c> value.</param>
/// <param name="TakeCount">The <c>$top</c> value; the maximum integer when not limited.</param>
public record SelectOperation(
    LambdaExpression Filter,
    ImmutableArray<SelectOrder> Orders,
    int SkipCount,
    int TakeCount)
{
    /// <summary>Returns an operation that keeps only the filter.</summary>
    public SelectOperation ToCountOperation() => Default with { Filter = this.Filter };

    public virtual bool Equals(SelectOperation? other) =>
        ReferenceEquals(this, other)

        || (other is not null

            && this.Filter == other.Filter
            && this.Orders.SequenceEqual(other.Orders)
            && this.SkipCount == other.SkipCount
            && this.TakeCount == other.TakeCount);

    public override int GetHashCode() => this.Orders.Length ^ this.SkipCount ^ this.TakeCount;

    /// <summary>Creates an operation with a filter built from a LINQ expression and no ordering or paging.</summary>
    /// <param name="filter">The filter.</param>
    /// <typeparam name="TSource">The filtered type.</typeparam>
    public static SelectOperation CreateFilter<TSource>(System.Linq.Expressions.Expression<Func<TSource, bool>> filter) => CreateFilter((System.Linq.Expressions.LambdaExpression)filter);

    /// <summary>Creates an operation with a filter built from a LINQ lambda and no ordering or paging.</summary>
    /// <param name="filter">The filter.</param>
    public static SelectOperation CreateFilter(System.Linq.Expressions.LambdaExpression filter) => new(new LambdaExpression(filter), Default.Orders, Default.SkipCount, Default.TakeCount);

    /// <summary>The operation that selects everything: <c>true</c> filter, no ordering, no skip, no limit.</summary>
    public static SelectOperation Default { get; } =

        new(new LambdaExpression(BooleanConstantExpression.True, [ParameterExpression.Default]), [], 0, int.MaxValue);
}
