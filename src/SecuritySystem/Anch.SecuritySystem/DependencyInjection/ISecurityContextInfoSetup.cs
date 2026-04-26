using System.Linq.Expressions;

using Anch.HierarchicalExpand;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DependencyInjection;

public interface ISecurityContextInfoSetup<TSecurityContext>
	where TSecurityContext : class, ISecurityContext
{
    ISecurityContextInfoSetup<TSecurityContext> SetName(string name);

    ISecurityContextInfoSetup<TSecurityContext> SetDisplayFunc(Func<TSecurityContext, string> displayFunc);

    ISecurityContextInfoSetup<TSecurityContext> SetIdentityPath<TSecurityContextIdent>(Expression<Func<TSecurityContext, TSecurityContextIdent>> identityPath)
        where TSecurityContextIdent : struct;

    ISecurityContextInfoSetup<TSecurityContext> SetHierarchicalInfo(
        HierarchicalInfo<TSecurityContext> hierarchicalInfo,
        FullAncestorLinkInfo<TSecurityContext> fullAncestorLinkInfo,
        DeepLevelInfo<TSecurityContext>? deepLevelInfo = null);

    ISecurityContextInfoSetup<TSecurityContext> SetHierarchicalInfo<TDirectedLink, TUndirectedLink>(
        Expression<Func<TSecurityContext, TSecurityContext?>> parentPath,
        AncestorLinkInfo<TSecurityContext, TDirectedLink> directed,
        AncestorLinkInfo<TSecurityContext, TUndirectedLink> undirected,
        Expression<Func<TSecurityContext, int>>? deepLevelPath = null) =>
        this.SetHierarchicalInfo(
            new HierarchicalInfo<TSecurityContext>(parentPath),
            new FullAncestorLinkInfo<TSecurityContext, TDirectedLink, TUndirectedLink>(directed, undirected),
            deepLevelPath == null ? null : new DeepLevelInfo<TSecurityContext>(deepLevelPath));

    ISecurityContextInfoSetup<TSecurityContext> AddExtension(Action<IServiceCollection> extension);
}