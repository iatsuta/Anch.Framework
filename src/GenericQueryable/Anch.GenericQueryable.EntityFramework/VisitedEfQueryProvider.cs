using System.Linq.Expressions;

using Anch.GenericQueryable.Services;

using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.EntityFramework;

/// <summary>The EF Core query provider used by GenericQueryable: it exposes the executor and applies the configured expression visitor to every query before execution.</summary>
/// <param name="queryCompiler">The EF query compiler.</param>
/// <param name="executor">Runs the ORM-independent operations.</param>
/// <param name="visitor">Optional visitor applied to every query.</param>
public class VisitedEfQueryProvider(
    IQueryCompiler queryCompiler,
    IGenericQueryableExecutor executor,
    [FromKeyedServices(nameof(GenericQueryable))]
    ExpressionVisitor? visitor = null)
    : EntityQueryProvider(queryCompiler), IGenericQueryProvider
{
    /// <inheritdoc/>
    public IGenericQueryableExecutor Executor { get; } = executor;

    //public override IQueryable CreateQuery(Expression expression) => base.CreateQuery(this.TryApplyVisitor(expression));

    //public override IQueryable<TElement> CreateQuery<TElement>(Expression expression) => base.CreateQuery<TElement>(this.TryApplyVisitor(expression));

    /// <inheritdoc/>
    public override object Execute(Expression expression) => base.Execute(this.TryApplyVisitor(expression));

    /// <inheritdoc/>
    public override TResult Execute<TResult>(Expression expression) => base.Execute<TResult>(this.TryApplyVisitor(expression));

    /// <inheritdoc/>
    public override TResult ExecuteAsync<TResult>(Expression expression, CancellationToken ct) =>
        base.ExecuteAsync<TResult>(this.TryApplyVisitor(expression), ct);

    private Expression TryApplyVisitor(Expression expression) => visitor == null ? expression : visitor.Visit(expression);
}