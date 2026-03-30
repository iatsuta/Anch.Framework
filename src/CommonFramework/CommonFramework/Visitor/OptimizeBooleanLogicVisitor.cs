using System.Linq.Expressions;

using CommonFramework.Maybe;

namespace CommonFramework.Visitor;

public class OptimizeBooleanLogicVisitor : ExpressionVisitor
{
    private OptimizeBooleanLogicVisitor()
    {
    }

    protected override Expression VisitConditional(ConditionalExpression node)
    {
        var visitedTest = this.Visit(node.Test);

        var evalReq =
            
            from testObj in visitedTest.GetDeepMemberConstValue<bool>()

            select this.Visit(testObj ? node.IfTrue : node.IfFalse);


        return evalReq.GetValueOrDefault(Expression () =>
        {
            var visitedIfTrue = this.Visit(node.IfTrue);
            var visitedIfFalse = this.Visit(node.IfFalse);

            if (visitedTest == node.Test && visitedIfTrue == node.IfTrue && visitedIfFalse == node.IfFalse)
            {
                return node;
            }
            else
            {
                return Expression.Condition(visitedTest, visitedIfTrue, visitedIfFalse, node.Type);
            }
        });
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        var left = this.Visit(node.Left);

        var leftValue = left.GetPureDeepMemberConstExpression();

        if (leftValue != null)
        {
            switch (node.NodeType)
            {
                case ExpressionType.AndAlso when !(bool)leftValue.Value!:
                    return Expression.Constant(false);
                case ExpressionType.OrElse when (bool)leftValue.Value!:
                    return Expression.Constant(true);
            }
        }

        var right = this.Visit(node.Right);
        var rightValue = right.GetPureDeepMemberConstExpression();

        if (leftValue != null && rightValue != null)
        {
            var methodResult = node.Method?.Invoke(null, [leftValue.Value, rightValue.Value])
                               ?? node.NodeType.GetBinaryMethod()?.Invoke(leftValue.Value!, rightValue.Value!);

            if (methodResult != null)
            {
                return Expression.Constant(methodResult);
            }
        }

        if (node.Method == null)
        {
            if (leftValue != null)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.AndAlso when (bool)leftValue.Value!:
                        return right;
                    case ExpressionType.OrElse when !(bool)leftValue.Value!:
                        return right;
                }
            }

            if (rightValue != null)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.AndAlso:
                        return (bool)rightValue.Value! ? left : Expression.Constant(false);
                    case ExpressionType.OrElse:
                        return (bool)rightValue.Value! ? Expression.Constant(true) : left;
                }
            }
        }

        if (left == node.Left && right == node.Right)
        {
            return node;
        }

        return Expression.MakeBinary(
            node.NodeType,
            left,
            right,
            node.IsLiftedToNull,
            node.Method,
            node.Conversion);
    }



    public static readonly OptimizeBooleanLogicVisitor Value = new();
}