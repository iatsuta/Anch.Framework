using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using Anch.Core.ExpressionComparers;

namespace Anch.Core;

public sealed record LambdaExpressionPath(ImmutableArray<LambdaExpression> Properties)
{
    public LambdaExpressionPath(IEnumerable<LambdaExpression> properties)
        : this([..properties])
    {
    }

    private static readonly IEqualityComparer<LambdaExpression> Comparer = ExpressionComparer.Default;

    private int? hashCode;

    public bool Equals(LambdaExpressionPath? other) =>

        ReferenceEquals(this, other)

        || (other is not null && this.Properties.SequenceEqual(other.Properties, Comparer));

    public override int GetHashCode()
    {
        return this.hashCode ??= this.ComputeHashCode();
    }

    private int ComputeHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.Properties.Length);

        foreach (var expr in this.Properties)
            hash.Add(expr, Comparer);

        return hash.ToHashCode();
    }

    public static LambdaExpressionPath Create(Type sourceType, string[] properties)
    {
        var typedProperties = properties.Scan(
            default(PropertyInfo),
            (prevProperty, propertyName) =>
            {
                var currentType = prevProperty == null ? sourceType : prevProperty.PropertyType.GetCollectionElementTypeOrSelf();

                return currentType.GetRequiredProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            }).Skip(1).Select(v => v!);

        return new LambdaExpressionPath(typedProperties.Select(v => v.ToGetLambdaExpression()));
    }
}