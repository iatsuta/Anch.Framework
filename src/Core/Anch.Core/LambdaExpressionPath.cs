using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using Anch.Core.ExpressionComparers;

namespace Anch.Core;

public sealed class LambdaExpressionPath(ImmutableArray<LambdaExpression> properties) : IEquatable<LambdaExpressionPath>
{
    private static readonly IEqualityComparer<LambdaExpression> Comparer = ExpressionComparer.Default;

    private int? hashCode;

    public LambdaExpressionPath(IEnumerable<LambdaExpression> properties)
        : this([.. properties])
    {
    }

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

    public static LambdaExpressionPath Create(Type sourceType, string[] properties)
    {
        var typedProperties = properties.Scan(
            default(PropertyInfo?),
            (prevProperty, propertyName) =>
            {
                var currentType = prevProperty == null ? sourceType : prevProperty.PropertyType.GetCollectionElementTypeOrSelf();

                return currentType.GetRequiredProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            }).Skip(1).Select(v => v!);

        return new LambdaExpressionPath(typedProperties.Select(v => v.ToGetLambdaExpression()));
    }
}