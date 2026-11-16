using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of constant expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
public class ConstantComparer : ExpressionComparer<ConstantExpression>
{
    protected override bool PureEquals(ConstantExpression x, ConstantExpression y)
    {
        return Equals(x.Value, y.Value);
    }
}
