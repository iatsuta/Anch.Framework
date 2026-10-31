using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionComparers;

namespace Anch.OData.Domain;

/// <summary>An ordering by a typed key.</summary>
/// <param name="Path">Selects the sort key.</param>
/// <typeparam name="TDomainObject">The type being ordered.</typeparam>
/// <typeparam name="TOrderKey">The type of the sort key.</typeparam>
public record SelectOrder<TDomainObject, TOrderKey>(Expression<Func<TDomainObject, TOrderKey>> Path) : SelectOrder<TDomainObject>
{
    private int? hashCode;

    [SetsRequiredMembers]
    protected SelectOrder(SelectOrder<TDomainObject, TOrderKey> source)
        : base(source)
    {
        this.Path = source.Path;
    }

    /// <inheritdoc/>
    public override IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) =>

        this.OrderType switch
        {
            OrderType.Asc => queryable.OrderBy(this.Path),

            OrderType.Desc => queryable.OrderByDescending(this.Path),

            _ => throw new ArgumentOutOfRangeException(nameof(this.OrderType))
        };

    /// <inheritdoc/>
    public override SelectOrder<TDomainObject> Visit(ExpressionVisitor visitor) => this with { Path = this.Path.UpdateBody(visitor) };

    public virtual bool Equals(SelectOrder<TDomainObject, TOrderKey>? other) =>
        object.ReferenceEquals(this, other) ||
        (other is not null
         && this.GetHashCode() == other.GetHashCode()
         && this.OrderType == other.OrderType
         && ExpressionComparer.Default.Equals(this.Path, other.Path));

    public override int GetHashCode() => this.hashCode ??= HashCode.Combine(ExpressionComparer.Default.GetHashCode(this.Path), this.OrderType);
}

/// <summary>One <c>$orderby</c> clause, independent of the key type.</summary>
/// <typeparam name="TDomainObject">The type being ordered.</typeparam>
public abstract record SelectOrder<TDomainObject> : IQueryableInjector<TDomainObject>
{
    /// <summary>The sort direction.</summary>
    public required OrderType OrderType { get; init; }

    /// <summary>Applies the ordering to a query.</summary>
    /// <param name="queryable">The query to order.</param>
    public abstract IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable);

    /// <summary>Rewrites the sort key with an expression visitor.</summary>
    /// <param name="visitor">The visitor to apply.</param>
    public abstract SelectOrder<TDomainObject> Visit(ExpressionVisitor visitor);
}