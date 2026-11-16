using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using Anch.Core.ExpressionComparers;

namespace Anch.Core;

/// <summary>A chain of property lambdas, such as <c>x =&gt; x.Orders</c> followed by <c>o =&gt; o.Items</c>, compared by structure. Used to describe eager loading paths.</summary>
/// <param name="properties">The lambdas of the chain, outermost first.</param>
public sealed class LambdaExpressionPath(ImmutableArray<LambdaExpression> properties) : IEquatable<LambdaExpressionPath>
{
    private static readonly IEqualityComparer<LambdaExpression> Comparer = ExpressionComparer.Default;

    private int? hashCode;

    public LambdaExpressionPath(IEnumerable<LambdaExpression> properties)
        : this([.. properties])
    {
    }

    /// <summary>The lambdas of the chain; each takes the result of the previous one (or an element of it, for collections).</summary>
    public ImmutableArray<LambdaExpression> Properties { get; } = properties;

    public override bool Equals(object? obj) => this.Equals(obj as LambdaExpressionPath);

    public bool Equals(LambdaExpressionPath? other) =>
        object.ReferenceEquals(this, other)
        || (other is not null
            && this.Properties.Length == other.Properties.Length
            && this.GetHashCode() == other.GetHashCode()
            && this.Properties.SequenceEqual(other.Properties, Comparer));

    public override int GetHashCode() => this.hashCode ??= this.ComputeHashCode();

    private int ComputeHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.Properties.Length);

        foreach (var expr in this.Properties)
            hash.Add(expr, Comparer);

        return hash.ToHashCode();
    }

    public static bool operator ==(LambdaExpressionPath? left, LambdaExpressionPath? right) =>
        object.ReferenceEquals(left, right) || (left is not null && left.Equals(right));

    public static bool operator !=(LambdaExpressionPath? left, LambdaExpressionPath? right) => !(left == right);

    /// <summary>Builds a path from property names, resolving each property on the type of the previous one (the element type for collections).</summary>
    /// <param name="sourceType">The type the first property belongs to.</param>
    /// <param name="properties">The property names, outermost first.</param>
    public static LambdaExpressionPath Create(Type sourceType, string[] properties)
    {
        var typedProperties = properties.Scan(
            default(PropertyInfo?),
            (prevProperty, propertyName) =>
            {
                var currentType = prevProperty is null ? sourceType : prevProperty.PropertyType.GetCollectionElementTypeOrSelf();

                return currentType.GetRequiredProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            }).Skip(1).Select(v => v!);

        return new LambdaExpressionPath(typedProperties.Select(v => v.ToGetLambdaExpression()));
    }
}