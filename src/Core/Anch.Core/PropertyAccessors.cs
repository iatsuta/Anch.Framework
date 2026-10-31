using System.Linq.Expressions;

using Anch.Core.ExpressionComparers;

namespace Anch.Core;

/// <summary>
/// A property described by a path expression together with a compiled getter and setter.
/// Two instances are equal when their path expressions are structurally equal, so they can be used as dictionary keys and in value-comparable records.
/// Create one from a lambda with <c>ToPropertyAccessors()</c>.
/// </summary>
/// <param name="Path">The property path, for example <c>x =&gt; x.Name</c>.</param>
/// <param name="Getter">Reads the property.</param>
/// <param name="Setter">Writes the property.</param>
/// <typeparam name="TSource">The type that owns the property.</typeparam>
/// <typeparam name="TProperty">The type of the property.</typeparam>
public record PropertyAccessors<TSource, TProperty>(
    Expression<Func<TSource, TProperty>> Path,
    Func<TSource, TProperty> Getter,
    Action<TSource, TProperty> Setter)
{
    private int? hashCode;

    /// <summary>Creates the accessors from the path; the getter and setter are derived from it.</summary>
    /// <param name="path">The property path.</param>
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