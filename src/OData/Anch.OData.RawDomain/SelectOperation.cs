using System.Collections.Immutable;

using Anch.OData.Domain.QueryLanguage;
using Anch.OData.Domain.QueryLanguage.Constant;

namespace Anch.OData.Domain;

public record SelectOperation(
    LambdaExpression Filter,
    ImmutableArray<SelectOrder> Orders,
    int SkipCount,
    int TakeCount)
{
    public SelectOperation ToCountOperation() => Default with { Filter = this.Filter };

    public virtual bool Equals(SelectOperation? other) =>
        ReferenceEquals(this, other)

        || (other is not null

            && this.Filter == other.Filter
            && this.Orders.SequenceEqual(other.Orders)
            && this.SkipCount == other.SkipCount
            && this.TakeCount == other.TakeCount);

    public override int GetHashCode() => this.Orders.Length ^ this.SkipCount ^ this.TakeCount;

    public static SelectOperation CreateFilter<TSource>(System.Linq.Expressions.Expression<Func<TSource, bool>> filter) => CreateFilter((System.Linq.Expressions.LambdaExpression)filter);

    public static SelectOperation CreateFilter(System.Linq.Expressions.LambdaExpression filter) => new(new LambdaExpression(filter), Default.Orders, Default.SkipCount, Default.TakeCount);

    public static SelectOperation Default { get; } =

        new(new LambdaExpression(BooleanConstantExpression.True, [ParameterExpression.Default]), [], 0, int.MaxValue);
}
