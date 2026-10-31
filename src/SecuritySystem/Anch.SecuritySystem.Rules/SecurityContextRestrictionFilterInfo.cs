using System.Linq.Expressions;

using Anch.Core.ExpressionComparers;
using Anch.Core.ExpressionEvaluate;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem;

/// <summary>Describes the filter that limits which objects of a security context type a role allows.</summary>
public abstract record SecurityContextRestrictionFilterInfo
{
    /// <summary>The security context type.</summary>
    public abstract Type SecurityContextType { get; }

    /// <summary>Returns the filter condition as an untyped lambda.</summary>
    /// <param name="serviceProvider">Supplies the services that the filter needs.</param>
    public abstract LambdaExpression GetBasePureFilter(IServiceProvider serviceProvider);
}

/// <summary>A typed filter description.</summary>
/// <typeparam name="TSecurityContext">The security context type.</typeparam>
public abstract record SecurityContextRestrictionFilterInfo<TSecurityContext> : SecurityContextRestrictionFilterInfo
    where TSecurityContext : ISecurityContext
{
    public override Type SecurityContextType { get; } = typeof(TSecurityContext);

    /// <summary>Returns the filter condition.</summary>
    /// <param name="serviceProvider">Supplies the services that the filter needs.</param>
    public abstract Expression<Func<TSecurityContext, bool>> GetPureFilter(IServiceProvider serviceProvider);

    public override LambdaExpression GetBasePureFilter(IServiceProvider serviceProvider) => this.GetPureFilter(serviceProvider);
}

/// <summary>A filter that is built from a service: the expression takes the service and returns the condition, so the condition can depend on registered data.</summary>
/// <param name="Expression">Builds the condition from the service.</param>
/// <typeparam name="TSecurityContext">The security context type.</typeparam>
/// <typeparam name="TService">The service the condition depends on.</typeparam>
public record SecurityContextRestrictionFilterInfo<TSecurityContext, TService>(
    Expression<Func<TService, Expression<Func<TSecurityContext, bool>>>> Expression)
    : SecurityContextRestrictionFilterInfo<TSecurityContext>
    where TSecurityContext : ISecurityContext
    where TService : notnull
{
    private int? hashCode;

    protected SecurityContextRestrictionFilterInfo(SecurityContextRestrictionFilterInfo<TSecurityContext, TService> source)
        : base(source)
    {
        this.Expression = source.Expression;
    }

    public virtual bool Equals(SecurityContextRestrictionFilterInfo<TSecurityContext, TService>? other) =>
        ReferenceEquals(this, other)
        || (other is not null
            && this.GetHashCode() == other.GetHashCode()
            && ExpressionComparer.Default.Equals(this.Expression, other.Expression));

    public override Expression<Func<TSecurityContext, bool>> GetPureFilter(IServiceProvider serviceProvider)
    {
        var service = serviceProvider.GetRequiredService<TService>();

        var expressionEvaluatorStorage = serviceProvider.GetRequiredService<IExpressionEvaluatorStorage>();

        var expressionEvaluator = expressionEvaluatorStorage.GetForType(typeof(SecurityContextRestrictionFilterInfo<TSecurityContext, TService>));

        return expressionEvaluator.Evaluate(this.Expression, service);
    }

    public override int GetHashCode() => this.hashCode ??= ExpressionComparer.Default.GetHashCode(this.Expression);
}