using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource;
using CommonFramework.VisualIdentitySource;

using SecuritySystem.Services;

namespace SecuritySystem.ExternalSystem.SecurityContextStorage;

public class PlainTypedSecurityContextStorage<TSecurityContext, TSecurityContextIdent>(
    IQueryableSource queryableSource,
    IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo,
    ISecurityIdentityConverter<TSecurityContextIdent> securityIdentityConverter,
    LocalStorage<TSecurityContext, TSecurityContextIdent> localStorage,
    IDomainObjectDisplayService displayService)
    : TypedSecurityContextStorageBase<TSecurityContext, TSecurityContextIdent>(queryableSource, identityInfo, securityIdentityConverter, localStorage)
    where TSecurityContext : class, ISecurityContext
    where TSecurityContextIdent : notnull
{
    protected override SecurityContextData<TSecurityContextIdent> CreateSecurityContextData(TSecurityContext securityContext) =>

        new(identityInfo.Id.Getter(securityContext), displayService.Format(securityContext), default);

    protected override IEnumerable<TSecurityContext> GetSecurityContextsWithMasterExpand(TSecurityContext startSecurityObject)
    {
        return [startSecurityObject];
    }
}