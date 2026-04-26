using Anch.GenericRepository;
using Anch.VisualIdentitySource;

namespace Anch.SecuritySystem.SecurityAccessor;

public class SecurityAccessorInfinityStorage<TUser>(
    IQueryableSource queryableSource,
    IVisualIdentityInfo<TUser> visualIdentityInfo)
    : ISecurityAccessorInfinityStorage
    where TUser : class
{
    public IEnumerable<string> GetInfinityData() => queryableSource.GetQueryable<TUser>().Select(visualIdentityInfo.Name.Path);
}