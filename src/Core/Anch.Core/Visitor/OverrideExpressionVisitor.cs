using System.Linq.Expressions;

namespace Anch.Core.Visitor;

public class OverrideExpressionVisitor(Func<Expression, bool> isReplaceExpression, Expression newExpression) : ExpressionVisitor
{
    public OverrideExpressionVisitor(Expression oldExpression, Expression newExpression)
        : this(node => node == oldExpression, newExpression)
    {
    }

    public override Expression? Visit(Expression? node)
    {
        return node != null && isReplaceExpression(node) ? newExpression : base.Visit(node);
    }
}