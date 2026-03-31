using System.Collections.Immutable;
using System.Linq.Expressions;

using CommonFramework;

using LambdaExpression = OData.Domain.QueryLanguage.LambdaExpression;

namespace OData.Domain;

public record SelectOperation<TDomainObject>(
    Expression<Func<TDomainObject, bool>> Filter,
    ImmutableArray<SelectOrder<TDomainObject>> Orders,
    int SkipCount,
    int TakeCount) : IDynamicSelectOperation, IQueryableInjector<TDomainObject>
{
    public bool HasPaging => this.SkipCount != 0 || this.TakeCount != 0;

    public ImmutableArray<LambdaExpression> Expands { get; init; } = [];

    public ImmutableArray<LambdaExpression> Selects { get; init; } = [];

    public SelectOperation<TDomainObject> WithoutPaging() => this.HasPaging ? this with { SkipCount = 0, TakeCount = 0 } : this;

    public SelectOperation<TDomainObject> AddFilter(Expression<Func<TDomainObject, bool>> filter) =>

        this with { Filter = this.Filter.BuildAnd(filter) };

    public SelectOperation<TDomainObject> AddOrder<TOrderKey>(Expression<Func<TDomainObject, TOrderKey>> path, OrderType type) =>

        this with { Orders = [..this.Orders, new SelectOrder<TDomainObject, TOrderKey>(path) { OrderType = type }] };

    public SelectOperation<TDomainObject> ToCountOperation() => Default with { Filter = this.Filter };

    public SelectOperation<TDomainObject> Visit(ExpressionVisitor visitor)
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


    public static readonly SelectOperation<TDomainObject> Default = new(_ => true, [], 0, int.MaxValue);
}
