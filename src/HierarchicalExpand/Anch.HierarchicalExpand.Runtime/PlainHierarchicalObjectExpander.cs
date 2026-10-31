using System.Linq.Expressions;

namespace Anch.HierarchicalExpand;

/// <summary>The expander of non-hierarchical types: returns the identifiers unchanged, whatever the expansion type.</summary>
/// <typeparam name="TIdent">The type of the identifier.</typeparam>
public class PlainHierarchicalObjectExpander<TIdent> : IHierarchicalObjectExpander<TIdent>
    where TIdent : notnull
{
    /// <inheritdoc/>
    public IEnumerable<TIdent> Expand(IEnumerable<TIdent> idents, HierarchicalExpandType _)
    {
        return idents;
    }

    /// <inheritdoc/>
    public Expression<Func<IEnumerable<TIdent>, IEnumerable<TIdent>>> GetExpandExpression(HierarchicalExpandType _)
    {
        return v => v;
    }

    /// <inheritdoc/>
    public Expression<Func<TIdent, IEnumerable<TIdent>>>? TryGetSingleExpandExpression(HierarchicalExpandType _)
    {
        return null;
    }

    /// <inheritdoc/>
    public Dictionary<TIdent, TIdent> ExpandWithParents(IEnumerable<TIdent> idents, HierarchicalExpandType expandType)
    {
        return this.ExpandWithParentsImplementation(idents, expandType);
    }

    /// <inheritdoc/>
    public Dictionary<TIdent, TIdent> ExpandWithParents(IQueryable<TIdent> idents, HierarchicalExpandType expandType)
    {
        return this.ExpandWithParentsImplementation(idents, expandType);
    }

    /// <summary>Maps every identifier to the default value, since there are no parents.</summary>
    /// <param name="idents">The identifiers.</param>
    /// <param name="_">Ignored.</param>
    public Dictionary<TIdent, TIdent> ExpandWithParentsImplementation(IEnumerable<TIdent> idents, HierarchicalExpandType _)
    {
        return idents.ToDictionary(id => id, _ => default(TIdent)!);
    }
}