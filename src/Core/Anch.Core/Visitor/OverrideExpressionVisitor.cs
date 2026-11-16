using System.Linq.Expressions;

namespace Anch.Core.Visitor;

/// <summary>Replaces the sub-expressions that match a predicate with a new expression.</summary>
/// <param name="isReplaceExpression">Selects the expressions to replace.</param>
/// <param name="newExpression">The replacement.</param>
public class OverrideExpressionVisitor(Func<Expression, bool> isReplaceExpression, Expression newExpression) : ExpressionVisitor
{
    public OverrideExpressionVisitor(Expression oldExpression, Expression newExpression)
        : this(node => node == oldExpression, newExpression)
    {
    }

    public override Expression? Visit(Expression? node)
    {
        return node is not null && isReplaceExpression(node) ? newExpression : base.Visit(node);
    }
}