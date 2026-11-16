using System.Collections.Immutable;

using Anch.Core;
using Anch.Core.ExpressionComparers;

using SExpressions = System.Linq.Expressions;

namespace Anch.OData.Domain;

/// <summary>
/// A parsed OData query bound to a domain type: a filter, orderings and paging, all as LINQ expressions.
/// It is a value (equal when the expressions are structurally equal) and an <see cref="IQueryableInjector{T}"/>, so it can be applied to any query.
/// </summary>
/// <param name="Filter">The <c>$filter</c> condition.</param>
/// <param name="Orders">The <c>$orderby</c> clauses, in order.</param>
/// <param name="SkipCount">The <c>$skip</c> value, or the default value when paging is not requested.</param>
/// <param name="TakeCount">The <c>$top</c> value, or the default value when paging is not requested.</param>
/// <typeparam name="TDomainObject">The type the query is applied to.</typeparam>
public record SelectOperation<TDomainObject>(
    SExpressions.Expression<Func<TDomainObject, bool>> Filter,
    ImmutableArray<SelectOrder<TDomainObject>> Orders,
    int SkipCount,
    int TakeCount) : IQueryableInjector<TDomainObject>
{
    private int? hashCode;

    protected SelectOperation(SelectOperation<TDomainObject> source)
    {
        this.Filter = source.Filter;
        this.Orders = source.Orders;
        this.SkipCount = source.SkipCount;
        this.TakeCount = source.TakeCount;
    }

    /// <summary><c>true</c> if skip or take is set.</summary>
    public bool HasPaging => this.SkipCount != Default.SkipCount || this.TakeCount != Default.TakeCount;

    public virtual bool Equals(SelectOperation<TDomainObject>? other) =>
        object.ReferenceEquals(this, other) ||
        (other is not null
         && this.GetHashCode() == other.GetHashCode()
         && this.SkipCount == other.SkipCount
         && this.TakeCount == other.TakeCount
         && ExpressionComparer.Default.Equals(this.Filter, other.Filter) && this.Orders.SequenceEqual(other.Orders));

    public override int GetHashCode() => this.hashCode ??= HashCode.Combine(ExpressionComparer.Default.GetHashCode(this.Filter),
        this.Orders.Aggregate(0, (hash, order) => HashCode.Combine(hash, order.GetHashCode())), this.SkipCount, this.TakeCount);

    /// <summary>Returns the operation with skip and take removed, for example to count all matching items.</summary>
    public SelectOperation<TDomainObject> WithoutPaging() => this.HasPaging ? this with { SkipCount = Default.SkipCount, TakeCount = Default.TakeCount } : this;

    /// <summary>Adds a condition; both the existing filter and the new one must hold.</summary>
    /// <param name="filter">The condition to add.</param>
    public SelectOperation<TDomainObject> AddFilter(SExpressions.Expression<Func<TDomainObject, bool>> filter) =>

        this with { Filter = this.Filter.BuildAnd(filter) };

    /// <summary>Appends an ordering.</summary>
    /// <param name="path">Selects the sort key.</param>
    /// <param name="type">The sort direction.</param>
    /// <typeparam name="TOrderKey">The type of the sort key.</typeparam>
    public SelectOperation<TDomainObject> AddOrder<TOrderKey>(SExpressions.Expression<Func<TDomainObject, TOrderKey>> path, OrderType type) =>

        this with { Orders = [.. this.Orders, new SelectOrder<TDomainObject, TOrderKey>(path) { OrderType = type }] };

    /// <summary>Returns an operation that keeps only the filter, which is what a count query needs.</summary>
    public SelectOperation<TDomainObject> ToCountOperation() => Default with { Filter = this.Filter };

    /// <summary>Rewrites the filter and the order keys with an expression visitor.</summary>
    /// <param name="visitor">The visitor to apply.</param>
    public SelectOperation<TDomainObject> Visit(SExpressions.ExpressionVisitor visitor)
    {
        var newFilter = this.Filter.UpdateBody(visitor);

        var newOrders = this.Orders.Select(order => order.Visit(visitor));

        return this with { Filter = newFilter, Orders = [.. newOrders] };
    }

    /// <inheritdoc/>
    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> baseQueryable) =>
        this.GetProcessQueryableElements().Aggregate(baseQueryable, (q, f) => f(q));

    private IEnumerable<Func<IQueryable<TDomainObject>, IQueryable<TDomainObject>>> GetProcessQueryableElements()
    {
        yield return q => q.Where(this.Filter);

        yield return q => this.Orders.Aggregate(q, (query, order) => order.Inject(query));

        if (this.SkipCount != Default.SkipCount)
        {
            yield return q => q.Skip(this.SkipCount);
        }

        if (this.TakeCount != Default.TakeCount)
        {
            yield return q => q.Take(this.TakeCount);
        }
    }


    /// <summary>The operation that selects everything: no filter, no ordering, no paging.</summary>
    public static readonly SelectOperation<TDomainObject> Default = new(_ => true, [], SelectOperation.Default.SkipCount, SelectOperation.Default.TakeCount);
}