using System.Collections;
using System.Linq.Expressions;

namespace Anch.HierarchicalExpand;

/// <summary>
/// Expands identifiers of hierarchical objects to the identifiers of their ancestors and/or descendants,
/// using the stored ancestor links, so the expansion can be done inside a database query without recursion.
/// </summary>
/// <typeparam name="TIdent">Type of the object identifier.</typeparam>
public interface IHierarchicalObjectExpander<TIdent> : IHierarchicalObjectExpander
    where TIdent : notnull
{
    /// <summary>Expands the identifiers in memory.</summary>
    /// <param name="idents">The identifiers to start from.</param>
    /// <param name="expandType">Which relatives to include (see <see cref="HierarchicalExpandType"/>).</param>
    /// <returns>The start identifiers together with the identifiers of the requested relatives.</returns>
    IEnumerable<TIdent> Expand(IEnumerable<TIdent> idents, HierarchicalExpandType expandType);

    /// <summary>Returns an expression that performs the expansion; it can be embedded into a query so the work is done in SQL.</summary>
    /// <param name="expandType">Which relatives to include.</param>
    Expression<Func<IEnumerable<TIdent>, IEnumerable<TIdent>>> GetExpandExpression(HierarchicalExpandType expandType);

    /// <summary>Returns an expression that expands a single identifier.</summary>
    /// <param name="expandType">Which relatives to include.</param>
    /// <returns>The expression, or <c>null</c> when it is not supported (for example for a non-hierarchical type).</returns>
    Expression<Func<TIdent, IEnumerable<TIdent>>>? TryGetSingleExpandExpression(HierarchicalExpandType expandType);

    /// <summary>Expands the identifiers and reports, for every result, which start identifier it was reached from.</summary>
    /// <param name="idents">The identifiers to start from.</param>
    /// <param name="expandType">Which relatives to include.</param>
    /// <returns>A dictionary from the expanded identifier to the start identifier it comes from.</returns>
    Dictionary<TIdent, TIdent> ExpandWithParents(IEnumerable<TIdent> idents, HierarchicalExpandType expandType);

    /// <summary>Expands the identifiers of a query and reports, for every result, which start identifier it was reached from.</summary>
    /// <param name="idents">The query that yields the identifiers to start from.</param>
    /// <param name="expandType">Which relatives to include.</param>
    /// <returns>A dictionary from the expanded identifier to the start identifier it comes from.</returns>
    Dictionary<TIdent, TIdent> ExpandWithParents(IQueryable<TIdent> idents, HierarchicalExpandType expandType);

    IEnumerable IHierarchicalObjectExpander.Expand(IEnumerable idents, HierarchicalExpandType expandType) =>
        this.Expand((IEnumerable<TIdent>)idents, expandType);
}

/// <summary>Untyped view of <see cref="IHierarchicalObjectExpander{TIdent}"/>.</summary>
public interface IHierarchicalObjectExpander
{
    /// <summary>Expands identifiers given as a non-generic sequence.</summary>
    /// <param name="idents">The identifiers to start from; their element type must match the expander's identifier type.</param>
    /// <param name="expandType">Which relatives to include.</param>
    IEnumerable Expand(IEnumerable idents, HierarchicalExpandType expandType);
}