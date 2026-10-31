using System.Linq.Expressions;

namespace Anch.RelativePath;

/// <summary>
/// Describes how to get from objects of one type to related objects of another type (a property or a collection property) as an expression,
/// so a condition on the related objects can be turned into a condition on the source objects and evaluated in a query.
/// Register with <c>AddRelativeDomainPath</c>.
/// </summary>
/// <typeparam name="TFrom">The source type.</typeparam>
/// <typeparam name="TTo">The related type.</typeparam>
public interface IRelativeDomainPathInfo<TFrom, TTo>
{
    /// <summary>Rebases the path onto another source type that can reach the original one.</summary>
    /// <param name="selector">Selects the original source object from the new one.</param>
    /// <typeparam name="TNewFrom">The new source type.</typeparam>
    IRelativeDomainPathInfo<TNewFrom, TTo> OverrideInput<TNewFrom>(Expression<Func<TNewFrom, TFrom>> selector);

    /// <summary>Extends the path with one more property of the related object.</summary>
    /// <param name="selector">Selects the next object.</param>
    /// <typeparam name="TNewTo">The new related type.</typeparam>
    IRelativeDomainPathInfo<TFrom, TNewTo> Select<TNewTo>(Expression<Func<TTo, TNewTo>> selector);

    /// <summary>Extends the path with a collection property of the related object.</summary>
    /// <param name="selector">Selects the next objects.</param>
    /// <typeparam name="TNewTo">The new related type.</typeparam>
    IRelativeDomainPathInfo<TFrom, TNewTo> Select<TNewTo>(Expression<Func<TTo, IEnumerable<TNewTo>>> selector);

    /// <summary>Turns a condition on the related objects into a condition on the source object: it holds when the related object (or, for collections, any related object) satisfies the filter.</summary>
    /// <param name="filter">The condition on the related objects.</param>
    Expression<Func<TFrom, bool>> CreateCondition(Expression<Func<TTo, bool>> filter);

    /// <summary>Returns the related objects of an object in memory.</summary>
    /// <param name="source">The source object.</param>
    IEnumerable<TTo> GetRelativeObjects(TFrom source);
}
