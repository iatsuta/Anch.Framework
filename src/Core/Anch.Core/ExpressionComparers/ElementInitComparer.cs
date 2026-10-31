using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of collection initializer elements.</summary>
/// <param name="rootComparer">The comparer used for the child expressions.</param>
public class ElementInitComparer(ExpressionComparer rootComparer) : IEqualityComparer<ElementInit>
{
    public bool Equals(ElementInit? x, ElementInit? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return x.AddMethod == y.AddMethod
               && x.Arguments.SequenceEqual(y.Arguments, rootComparer);
    }

    public int GetHashCode(ElementInit obj)
    {
        return obj.AddMethod.GetHashCode();
    }
}
