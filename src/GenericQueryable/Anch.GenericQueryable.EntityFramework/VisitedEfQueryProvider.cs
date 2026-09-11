using System.Linq.Expressions;

using Anch.GenericQueryable.Services;

using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.EntityFramework;

public class VisitedEfQueryProvider(
    IQueryCompiler queryCompiler,
    IGenericQueryableExecutor executor,
    [FromKeyedServices(nameof(GenericQueryable))]
    ExpressionVisitor? visitor = null)
    : EntityQueryProvider(queryCompiler), IGenericQueryProvider
{
    public IGenericQueryableExecutor Executor { get; } = executor;

    //public override IQueryable CreateQuery(Expression expression) => base.CreateQuery(this.TryApplyVisitor(expression));

    //public override IQueryable<TElement> CreateQuery<TElement>(Expression expression) => base.CreateQuery<TElement>(this.TryApplyVisitor(expression));

    public override object Execute(Expression expression) => base.Execute(this.TryApplyVisitor(expression));

    public override TResult Execute<TResult>(Expression expression) => base.Execute<TResult>(this.TryApplyVisitor(expression));

    public override TResult ExecuteAsync<TResult>(Expression expression, CancellationToken ct) =>
        base.ExecuteAsync<TResult>(this.TryApplyVisitor(expression), ct);

    private Expression TryApplyVisitor(Expression expression) => visitor == null ? expression : visitor.Visit(expression);
}