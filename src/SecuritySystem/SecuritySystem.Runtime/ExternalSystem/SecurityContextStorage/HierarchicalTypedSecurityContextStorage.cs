using CommonFramework;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource;
using CommonFramework.VisualIdentitySource;

using HierarchicalExpand;
using SecuritySystem.Services;

namespace SecuritySystem.ExternalSystem.SecurityContextStorage;

public class HierarchicalTypedSecurityContextStorage<TSecurityContext, TSecurityContextIdent>(
	IQueryableSource queryableSource,
    IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo,
    ISecurityIdentityConverter<TSecurityContextIdent> securityIdentityConverter,
    LocalStorage<TSecurityContext, TSecurityContextIdent> localStorage,
	IDomainObjectDisplayService displayService,
	HierarchicalInfo<TSecurityContext> hierarchicalInfo)
	: TypedSecurityContextStorageBase<TSecurityContext, TSecurityContextIdent>(queryableSource, identityInfo, securityIdentityConverter, localStorage)
	where TSecurityContext : class, ISecurityContext
	where TSecurityContextIdent : notnull
{
	protected override SecurityContextData<TSecurityContextIdent> CreateSecurityContextData(TSecurityContext securityContext) =>

		new(identityInfo.Id.Getter(securityContext), displayService.Format(securityContext),
			hierarchicalInfo.ParentFunc(securityContext).Maybe(identityInfo.Id.Getter));

	protected override IEnumerable<TSecurityContext> GetSecurityContextsWithMasterExpand(TSecurityContext startSecurityObject)
	{
		return startSecurityObject.GetAllElements(hierarchicalInfo.ParentFunc);
	}
}