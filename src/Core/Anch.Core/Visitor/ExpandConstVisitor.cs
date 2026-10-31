using System.Linq.Expressions;

namespace Anch.Core.Visitor;

/// <summary>Replaces closure field accesses with their current values as constants, so the tree no longer depends on captured variables.</summary>
public class ExpandConstVisitor : ExpressionVisitor
{
    public override Expression? Visit(Expression? baseNode)
    {
        var baseVisited = base.Visit(baseNode);

        var request =

            from node in baseVisited.ToMaybe()

            from res in node.GetConstantExpression()

            where res != node

            where res.Value is not IQueryable

            select res;

        return request.GetValueOrDefault(baseVisited);
    }

    public static readonly ExpandConstVisitor Value = new();
}