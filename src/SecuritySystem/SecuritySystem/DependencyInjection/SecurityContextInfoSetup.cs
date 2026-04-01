using System.Linq.Expressions;

using CommonFramework.IdentitySource.DependencyInjection;
using CommonFramework.VisualIdentitySource.DependencyInjection;

using HierarchicalExpand;
using HierarchicalExpand.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace SecuritySystem.DependencyInjection;

public class SecurityContextInfoSetup<TSecurityContext>(TypedSecurityIdentity identity) : ISecurityContextInfoSetup<TSecurityContext>
	where TSecurityContext : class, ISecurityContext
{
	private readonly List<Action<IServiceCollection>> extensions = new();

	private string name = typeof(TSecurityContext).Name;

	public Action<IHierarchicalExpandSetup>? HierarchicalSetupAction { get; private set; }

	public Action<IIdentitySourceSetup>? IdentitySetupAction { get; private set; }

	public Action<IVisualIdentitySourceSetup>? VisualIdentitySetupAction { get; private set; }

	public ISecurityContextInfoSetup<TSecurityContext> SetName(string newName)
	{
		this.name = newName;

		return this;
	}

	public ISecurityContextInfoSetup<TSecurityContext> SetDisplayFunc(Func<TSecurityContext, string> displayFunc)
	{
		this.VisualIdentitySetupAction = s => s.SetDisplay(displayFunc);

		return this;
	}

	public ISecurityContextInfoSetup<TSecurityContext> SetIdentityPath<TSecurityContextIdent>(Expression<Func<TSecurityContext, TSecurityContextIdent>> identityPath)
		where TSecurityContextIdent : struct
	{
		this.IdentitySetupAction = s => s.SetId(identityPath);

		return this;
	}

	public ISecurityContextInfoSetup<TSecurityContext> SetHierarchicalInfo(
		HierarchicalInfo<TSecurityContext> newHierarchicalInfo,
		FullAncestorLinkInfo<TSecurityContext> newFullAncestorLinkInfo,
        DeepLevelInfo<TSecurityContext>? deepLevelInfo = null)
	{
		this.HierarchicalSetupAction = s => s.AddHierarchicalInfo(newHierarchicalInfo, newFullAncestorLinkInfo, deepLevelInfo);

		return this;
	}

	public ISecurityContextInfoSetup<TSecurityContext> AddExtension(Action<IServiceCollection> extension)
	{
		this.extensions.Add(extension);

		return this;
	}

	public void Register(IServiceCollection services)
	{
		var securityContextInfo = new SecurityContextInfo<TSecurityContext>(identity, this.name);

		services.AddSingleton(securityContextInfo);
		services.AddSingleton<SecurityContextInfo>(securityContextInfo);

		foreach (var extension in this.extensions)
		{
			extension(services);
		}
	}
}