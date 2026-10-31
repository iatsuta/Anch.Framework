using Anch.GenericRepository;
using Anch.VisualIdentitySource;

namespace Anch.SecuritySystem.SecurityAccessor;

/// <summary>Supplies all users of the user entity type as the accessors with unrestricted access.</summary>
public class SecurityAccessorInfinityStorage<TUser>(
    IQueryableSource queryableSource,
    IVisualIdentityInfo<TUser> visualIdentityInfo)
    : ISecurityAccessorInfinityStorage
    where TUser : class
{
    public IEnumerable<string> GetInfinityData() => queryableSource.GetQueryable<TUser>().Select(visualIdentityInfo.Name.Path);
}