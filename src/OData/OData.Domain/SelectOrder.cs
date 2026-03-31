using System.Linq.Expressions;

using CommonFramework;

namespace OData.Domain;

public record SelectOrder<TDomainObject, TOrderKey>(Expression<Func<TDomainObject, TOrderKey>> Path) : SelectOrder<TDomainObject>
{
    public override LambdaExpression BasePath => this.Path;

    public override IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) =>

        this.OrderType switch
        {
            OrderType.Asc => queryable.OrderBy(this.Path),

            OrderType.Desc => queryable.OrderByDescending(this.Path),

            _ => throw new ArgumentOutOfRangeException(nameof(this.OrderType))
        };

    public override SelectOrder<TDomainObject> Visit(ExpressionVisitor visitor) => this with { Path = this.Path.UpdateBody(visitor) };
}

public abstract record SelectOrder<TDomainObject> : IQueryableInjector<TDomainObject>
{

    public abstract LambdaExpression BasePath { get; }

    public required OrderType OrderType { get; init; }

    public abstract IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable);

    public abstract SelectOrder<TDomainObject> Visit(ExpressionVisitor visitor);
}
