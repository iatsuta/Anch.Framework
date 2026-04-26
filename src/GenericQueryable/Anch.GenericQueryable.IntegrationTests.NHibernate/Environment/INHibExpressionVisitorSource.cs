using System.Linq.Expressions;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public interface INHibExpressionVisitorSource
{
    ExpressionVisitor Visitor { get; }
}