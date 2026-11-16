using System.Linq.Expressions;
using System.Reflection;

using Anch.GenericQueryable.IntegrationTests.Domain;

namespace Anch.GenericQueryable.IntegrationTests.Visitors;

public sealed class LinkIdVisitor : ExpressionVisitor
{
    private static readonly PropertyInfo LinkId = typeof(TestObject).GetProperty(nameof(TestObject.LinkId))!;
    private static readonly PropertyInfo Id = typeof(TestObject).GetProperty(nameof(TestObject.Id))!;

    protected override Expression VisitMember(MemberExpression node) =>
        node.Expression is { } target && node.Member.MetadataToken == LinkId.MetadataToken && node.Member.Module == LinkId.Module
            ? Expression.Property(Visit(target), Id)
            : base.VisitMember(node);
}