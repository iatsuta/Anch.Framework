using System.Linq.Expressions;

using Anch.Core.ExpressionComparers;

namespace Anch.SecuritySystem;

/// <summary>A condition on an object related to the secured one; used by <see cref="DomainSecurityRule.RelativeConditionSecurityRule"/> and <see cref="SecurityPathRestriction"/>.</summary>
public abstract record RelativeConditionInfo
{
    /// <summary>The type of the related object the condition is applied to.</summary>
    public abstract Type RelativeDomainObjectType { get; }
}

/// <summary>A typed condition; equality compares the expression structurally.</summary>
/// <param name="Condition">The condition on the related object.</param>
/// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
public record RelativeConditionInfo<TRelativeDomainObject>(Expression<Func<TRelativeDomainObject, bool>> Condition)
    : RelativeConditionInfo
{
    private int? hashCode;

    protected RelativeConditionInfo(RelativeConditionInfo<TRelativeDomainObject> source)
        : base(source)
    {
        this.Condition = source.Condition;
        this.RelativeDomainObjectType = source.RelativeDomainObjectType;
    }

    public override Type RelativeDomainObjectType { get; } = typeof(TRelativeDomainObject);

    public virtual bool Equals(RelativeConditionInfo<TRelativeDomainObject>? other) =>
        ReferenceEquals(this, other)
        || (other is not null
            && this.GetHashCode() == other.GetHashCode()
            && ExpressionComparer.Default.Equals(this.Condition, other.Condition));

    public override int GetHashCode() => this.hashCode ??= ExpressionComparer.Default.GetHashCode(this.Condition);
}