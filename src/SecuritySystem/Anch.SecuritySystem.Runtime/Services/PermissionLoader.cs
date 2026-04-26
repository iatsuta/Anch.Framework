using Anch.Core;
using Anch.GenericQueryable;
using Anch.GenericRepository;

namespace Anch.SecuritySystem.Services;

public class PermissionLoader<TPrincipal, TPermission>(
    IQueryableSource queryableSource,
    PermissionBindingInfo<TPermission, TPrincipal> bindingInfo) : IPermissionLoader<TPrincipal, TPermission>
    where TPrincipal : class
    where TPermission : class
{
    public IAsyncEnumerable<TPermission> LoadAsync(TPrincipal principal)
    {
        return queryableSource
            .GetQueryable<TPermission>()
            .Where(bindingInfo.Principal.Path.Select(p => p == principal))
            .GenericAsAsyncEnumerable();
    }
}