using Anch.IdentitySource;

namespace Anch.SecuritySystem.ExternalSystem.SecurityContextStorage;

public class LocalStorage<TSecurityContext, TSecurityContextIdent>(IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
	where TSecurityContext : ISecurityContext
	where TSecurityContextIdent : notnull
{
	private readonly HashSet<TSecurityContext> items = [];

	public bool IsExists(TSecurityContextIdent securityEntityId)
	{
		return this.items.Select(identityInfo.Id.Getter).Contains(securityEntityId);
	}

	public bool Register(TSecurityContext securityContext)
	{
		return this.items.Add(securityContext);
	}
}