using Anch.GenericRepository;
using Anch.IdentitySource;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.ExternalSystem.SecurityContextStorage;

public abstract class TypedSecurityContextStorageBase<TSecurityContext, TSecurityContextIdent>(
	IQueryableSource queryableSource,
    IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo,
	ISecurityIdentityConverter<TSecurityContextIdent> securityIdentityConverter,
    LocalStorage<TSecurityContext, TSecurityContextIdent> localStorage)
	: ITypedSecurityContextStorage<TSecurityContextIdent>
	where TSecurityContext : class, ISecurityContext
	where TSecurityContextIdent : notnull
{
	protected abstract SecurityContextData<TSecurityContextIdent> CreateSecurityContextData(TSecurityContext securityContext);

	public IEnumerable<SecurityContextData<TSecurityContextIdent>> GetSecurityContexts()
	{
		return queryableSource.GetQueryable<TSecurityContext>().ToList().Select(this.CreateSecurityContextData);
	}

	public IEnumerable<SecurityContextData<TSecurityContextIdent>> GetSecurityContextsByIdents(IEnumerable<TSecurityContextIdent> preSecurityEntityIdents)
	{
		var filter = identityInfo.CreateFilter(preSecurityEntityIdents);

		return queryableSource.GetQueryable<TSecurityContext>().Where(filter).ToList().Select(this.CreateSecurityContextData);
	}

	public IEnumerable<SecurityContextData<TSecurityContextIdent>> GetSecurityContextsWithMasterExpand(TSecurityContextIdent startSecurityEntityId)
	{
		var filter = identityInfo.CreateFilter(startSecurityEntityId);

		var securityObject = queryableSource.GetQueryable<TSecurityContext>().Single(filter);

		return this.GetSecurityContextsWithMasterExpand(securityObject).Select(this.CreateSecurityContextData);
	}

	public bool IsExists(TSecurityContextIdent securityContextId)
	{
		var filter = identityInfo.CreateFilter(securityContextId);

		return localStorage.IsExists(securityContextId)
		       || queryableSource.GetQueryable<TSecurityContext>().Any(filter);
	}

	protected abstract IEnumerable<TSecurityContext> GetSecurityContextsWithMasterExpand(TSecurityContext startSecurityObject);

	IEnumerable<SecurityContextData<object>> ITypedSecurityContextStorage.GetSecurityContextsByIdents(Array securityContextIdents)
	{
		return this.GetSecurityContextsByIdents(securityContextIdents.Cast<TSecurityContextIdent>()).Select(scd => scd.UpCast());
	}

    public bool IsExists(SecurityIdentity securityIdentity)
    {
        return this.IsExists(securityIdentityConverter.Convert(securityIdentity).Id);
    }

    IEnumerable<SecurityContextData<object>> ITypedSecurityContextStorage.GetSecurityContexts()
	{
		return this.GetSecurityContexts().Select(scd => scd.UpCast());
	}
}
