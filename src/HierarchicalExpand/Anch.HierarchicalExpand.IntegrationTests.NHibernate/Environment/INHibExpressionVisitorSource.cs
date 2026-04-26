using System.Linq.Expressions;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public interface INHibExpressionVisitorSource
{
    ExpressionVisitor Visitor { get; }
}