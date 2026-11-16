using System.Linq.Expressions;

using Anch.GenericQueryable.IntegrationTests.Visitors;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public class NHibExpressionVisitorSource : INHibExpressionVisitorSource
{
    public ExpressionVisitor Visitor { get; } = new LinkIdVisitor();
}