using System.Collections.Immutable;

using Anch.Core;
using Anch.Core.ExpressionComparers;

using SExpressions = System.Linq.Expressions;

namespace Anch.OData.Domain;

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

    public SelectOperation<TDomainObject> WithoutPaging() => this.HasPaging ? this with { SkipCount = Default.SkipCount, TakeCount = Default.TakeCount } : this;

    public SelectOperation<TDomainObject> AddFilter(SExpressions.Expression<Func<TDomainObject, bool>> filter) =>

        this with { Filter = this.Filter.BuildAnd(filter) };

    public SelectOperation<TDomainObject> AddOrder<TOrderKey>(SExpressions.Expression<Func<TDomainObject, TOrderKey>> path, OrderType type) =>

        this with { Orders = [.. this.Orders, new SelectOrder<TDomainObject, TOrderKey>(path) { OrderType = type }] };

    public SelectOperation<TDomainObject> ToCountOperation() => Default with { Filter = this.Filter };

    public SelectOperation<TDomainObject> Visit(SExpressions.ExpressionVisitor visitor)
    {
        var newFilter = this.Filter.UpdateBody(visitor);

        var newOrders = this.Orders.Select(order => order.Visit(visitor));

        return this with { Filter = newFilter, Orders = [.. newOrders] };
    }

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


    public static readonly SelectOperation<TDomainObject> Default = new(_ => true, [], SelectOperation.Default.SkipCount, SelectOperation.Default.TakeCount);
}