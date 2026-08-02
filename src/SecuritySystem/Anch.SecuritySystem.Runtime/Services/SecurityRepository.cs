using Anch.GenericQueryable;
using Anch.GenericRepository;

namespace Anch.SecuritySystem.Services;

public class SecurityRepository<TDomainObject>(IQueryableSource queryableSource, ISecurityIdentityFilterFactory<TDomainObject> filterFactory)
    : ISecurityRepository<TDomainObject>
    where TDomainObject : class
{
    public async Task<TDomainObject> GetObjectAsync(SecurityIdentity securityIdentity, CancellationToken ct)
    {
        var result = await queryableSource.GetQueryable<TDomainObject>().Where(filterFactory.CreateFilter(securityIdentity))
            .GenericSingleOrDefaultAsync(ct);

        return result ?? throw new ArgumentOutOfRangeException(nameof(securityIdentity),
            $"{typeof(TDomainObject).Name} with {nameof(securityIdentity)} '{securityIdentity}' not found");
    }
}