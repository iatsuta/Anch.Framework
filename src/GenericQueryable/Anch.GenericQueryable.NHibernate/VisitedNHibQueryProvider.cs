using System.Linq.Expressions;

using Anch.GenericQueryable.Services;

using NHibernate;
using NHibernate.Engine;
using NHibernate.Linq;

namespace Anch.GenericQueryable.NHibernate;

/// <summary>The NHibernate query provider used by GenericQueryable: it exposes the executor and applies the configured expression visitor to every query.</summary>
public class VisitedNHibQueryProvider : DefaultQueryProvider, IVisitedNHibQueryProvider
{
    /// <summary>Creates the provider for a session.</summary>
    /// <param name="session">The NHibernate session.</param>
    public VisitedNHibQueryProvider(ISessionImplementor session)
        : base(session)
    {
    }

    protected VisitedNHibQueryProvider(ISessionImplementor session, object collection, NhQueryableOptions options)
        : base(session, collection, options)
    {
    }

    /// <inheritdoc/>
    public ExpressionVisitor? Visitor { get; set; }

    /// <inheritdoc/>
    public IGenericQueryableExecutor Executor { get; set; } = null!;

    /// <inheritdoc/>
    protected override IQueryProvider CreateWithOptions(NhQueryableOptions options)
    {
        return new VisitedNHibQueryProvider(this.Session, this.Collection, options) { Visitor = this.Visitor, Executor = this.Executor };
    }

    private Expression TryApplyVisitor(Expression expression)
    {
        return this.Visitor is null ? expression : this.Visitor.Visit(expression);
    }

    /// <inheritdoc/>
    protected override NhLinqExpression PrepareQuery(Expression expression, out IQuery query)
    {
        return base.PrepareQuery(this.TryApplyVisitor(expression), out query);
    }

    /// <inheritdoc/>
    public override IQueryable CreateQuery(Expression expression)
    {
        return base.CreateQuery(this.TryApplyVisitor(expression));
    }

    /// <inheritdoc/>
    public override IQueryable<T> CreateQuery<T>(Expression expression)
    {
        return base.CreateQuery<T>(this.TryApplyVisitor(expression));
    }
}