using System.Linq.Expressions;

using Anch.Core.ExpressionComparers;

namespace Anch.Core;

public record PropertyAccessors<TSource, TProperty>(
    Expression<Func<TSource, TProperty>> Path,
    Func<TSource, TProperty> Getter,
    Action<TSource, TProperty> Setter)
{
    public PropertyAccessors(
        Expression<Func<TSource, TProperty>> path)
        : this(path, path.ToGetFunc(), path.ToLazySetAction())
    {
    }

    public virtual bool Equals(PropertyAccessors<TSource, TProperty>? other) =>
        object.ReferenceEquals(this, other)
        || (other is not null && ExpressionComparer.Default.Equals(this.Path, other.Path));

    public override int GetHashCode()
    {
        return ExpressionComparer.Default.GetHashCode(this.Path);
    }
}