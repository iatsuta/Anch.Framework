using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Structural comparer of parameter expression nodes; used by <see cref="ExpressionComparer"/>.</summary>
public class ParameterComparer : ExpressionComparer<ParameterExpression>
{
    protected override bool PureEquals(ParameterExpression x, ParameterExpression y)
    {
        return x.Name == y.Name;
    }
}