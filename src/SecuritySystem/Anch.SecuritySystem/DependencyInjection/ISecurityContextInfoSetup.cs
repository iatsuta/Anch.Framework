using System.Linq.Expressions;

using Anch.HierarchicalExpand;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DependencyInjection;

/// <summary>Fluent configuration of one security context type: name, display text, identifier and hierarchy.</summary>
/// <typeparam name="TSecurityContext">The security context type.</typeparam>
public interface ISecurityContextInfoSetup<TSecurityContext>
    where TSecurityContext : class, ISecurityContext
{
    /// <summary>Sets the name of the context type; the type name is used by default.</summary>
    /// <param name="name">The name.</param>
    ISecurityContextInfoSetup<TSecurityContext> SetName(string name);

    /// <summary>Sets how a context object is shown to people.</summary>
    /// <param name="displayFunc">Converts an object to its display text.</param>
    ISecurityContextInfoSetup<TSecurityContext> SetDisplayFunc(Func<TSecurityContext, string> displayFunc);

    /// <summary>Declares the identifier property of the context type explicitly.</summary>
    /// <param name="identityPath">Selects the identifier property.</param>
    /// <typeparam name="TSecurityContextIdent">The type of the identifier.</typeparam>
    ISecurityContextInfoSetup<TSecurityContext> SetIdentityPath<TSecurityContextIdent>(Expression<Func<TSecurityContext, TSecurityContextIdent>> identityPath)
        where TSecurityContextIdent : struct;

    /// <summary>Declares that the context type is hierarchical, so granting access to a node can also cover its parents or children.</summary>
    /// <param name="hierarchicalInfo">The parent property.</param>
    /// <param name="fullAncestorLinkInfo">The directed and undirected ancestor links.</param>
    /// <param name="deepLevelInfo">The stored depth property; <c>null</c> if there is none.</param>
    ISecurityContextInfoSetup<TSecurityContext> SetHierarchicalInfo(
        HierarchicalInfo<TSecurityContext> hierarchicalInfo,
        FullAncestorLinkInfo<TSecurityContext> fullAncestorLinkInfo,
        DeepLevelInfo<TSecurityContext>? deepLevelInfo = null);

    /// <summary>Declares that the context type is hierarchical, from property expressions.</summary>
    /// <param name="parentPath">Selects the parent of an object.</param>
    /// <param name="directed">The directed ancestor links.</param>
    /// <param name="undirected">The undirected ancestor links.</param>
    /// <param name="deepLevelPath">Selects the stored depth property; <c>null</c> if there is none.</param>
    /// <typeparam name="TDirectedLink">The directed link entity type.</typeparam>
    /// <typeparam name="TUndirectedLink">The undirected link entity type.</typeparam>
    ISecurityContextInfoSetup<TSecurityContext> SetHierarchicalInfo<TDirectedLink, TUndirectedLink>(
        Expression<Func<TSecurityContext, TSecurityContext?>> parentPath,
        AncestorLinkInfo<TSecurityContext, TDirectedLink> directed,
        AncestorLinkInfo<TSecurityContext, TUndirectedLink> undirected,
        Expression<Func<TSecurityContext, int>>? deepLevelPath = null) =>
        this.SetHierarchicalInfo(
            new HierarchicalInfo<TSecurityContext>(parentPath),
            new FullAncestorLinkInfo<TSecurityContext, TDirectedLink, TUndirectedLink>(directed, undirected),
            deepLevelPath is null ? null : new DeepLevelInfo<TSecurityContext>(deepLevelPath));

    /// <summary>Registers additional services together with the context type.</summary>
    /// <param name="extension">Registers the services.</param>
    ISecurityContextInfoSetup<TSecurityContext> AddExtension(Action<IServiceCollection> extension);
}