using System.Linq.Expressions;

using Anch.Core.ExpressionComparers;

namespace Anch.Core;

public record PropertyAccessors<TSource, TProperty>(
    Expression<Func<TSource, TProperty>> Path,
    Func<TSource, TProperty> Getter,
    Action<TSource, TProperty> Setter)
{
    private int? hashCode;

    public PropertyAccessors(
        Expression<Func<TSource, TProperty>> path)
        : this(path, path.ToGetFunc(), path.ToLazySetAction())
    {
    }

    protected PropertyAccessors(PropertyAccessors<TSource, TProperty> source)
    {
        this.Path = source.Path;
        this.Getter = source.Getter;
        this.Setter = source.Setter;
    }


    public virtual bool Equals(PropertyAccessors<TSource, TProperty>? other) =>
        ReferenceEquals(this, other)
        || (other is not null
            && this.GetHashCode() == other.GetHashCode()
            && ExpressionComparer.Default.Equals(this.Path, other.Path));

    public override int GetHashCode() => this.hashCode ??= ExpressionComparer.Default.GetHashCode(this.Path);
}