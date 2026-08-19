using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionComparers;

namespace Anch.OData.Domain;

public record SelectOrder<TDomainObject, TOrderKey>(Expression<Func<TDomainObject, TOrderKey>> Path) : SelectOrder<TDomainObject>
{
    private int? hashCode;

    [SetsRequiredMembers]
    protected SelectOrder(SelectOrder<TDomainObject, TOrderKey> source)
        : base(source)
    {
        this.Path = source.Path;
    }

    public override IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) =>

        this.OrderType switch
        {
            OrderType.Asc => queryable.OrderBy(this.Path),

            OrderType.Desc => queryable.OrderByDescending(this.Path),

            _ => throw new ArgumentOutOfRangeException(nameof(this.OrderType))
        };

    public override SelectOrder<TDomainObject> Visit(ExpressionVisitor visitor) => this with { Path = this.Path.UpdateBody(visitor) };

    public virtual bool Equals(SelectOrder<TDomainObject, TOrderKey>? other) =>
        object.ReferenceEquals(this, other) ||
        (other is not null
         && this.GetHashCode() == other.GetHashCode()
         && this.OrderType == other.OrderType
         && ExpressionComparer.Default.Equals(this.Path, other.Path));

    public override int GetHashCode() => this.hashCode ??= HashCode.Combine(ExpressionComparer.Default.GetHashCode(this.Path), this.OrderType);
}

public abstract record SelectOrder<TDomainObject> : IQueryableInjector<TDomainObject>
{
    public required OrderType OrderType { get; init; }

    public abstract IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable);

    public abstract SelectOrder<TDomainObject> Visit(ExpressionVisitor visitor);
}