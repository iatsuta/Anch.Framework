using CommonFramework;
using CommonFramework.GenericRepository;
using CommonFramework.VisualIdentitySource;

using GenericQueryable;
using SecuritySystem.ExternalSystem.Management;
using SecuritySystem.Services;
using SecuritySystem.UserSource;

using System.Collections.Immutable;

namespace SecuritySystem.GeneralPermission;

public class GeneralPrincipalSourceService<TPrincipal, TPermission>(
    PermissionBindingInfo<TPermission, TPrincipal> bindingInfo,
    IQueryableSource queryableSource,
    IAvailablePermissionSource<TPermission> availablePermissionSource,
    IManagedPrincipalHeaderConverter<TPrincipal> principalHeaderConverter,
    IManagedPrincipalConverter<TPrincipal> principalConverter,
    IUserQueryableSource<TPrincipal> userQueryableSource,
    IVisualIdentityInfo<TPrincipal> visualIdentity) : IPrincipalSourceService
    where TPrincipal : class
{
    public Type PrincipalType { get; } = typeof(TPrincipal);

    public IAsyncEnumerable<ManagedPrincipalHeader> GetPrincipalsAsync(string nameFilter, int limit)
    {
        return queryableSource.GetQueryable<TPrincipal>()
            .Pipe(
                !string.IsNullOrWhiteSpace(nameFilter),
                q => q.Where(visualIdentity.Name.Path.Select(principalName => principalName.Contains(nameFilter))))
            .Select(principalHeaderConverter.ConvertExpression)
            .GenericAsAsyncEnumerable();
    }

    public async ValueTask<ManagedPrincipal?> TryGetPrincipalAsync(UserCredential userCredential, CancellationToken cancellationToken)
    {
        var principal = await userQueryableSource.GetQueryable(userCredential).GenericSingleOrDefaultAsync(cancellationToken);

        if (principal is null)
        {
            return null;
        }
        else
        {
            return await principalConverter.ToManagedPrincipalAsync(principal, cancellationToken);
        }
    }

    public IAsyncEnumerable<string> GetLinkedPrincipalsAsync(ImmutableHashSet<SecurityRole> securityRoles)
    {
        var securityRule = new DomainSecurityRule.ExpandedRolesSecurityRule(securityRoles)
        {
            CustomCredential = new SecurityRuleCredential.AnyUserCredential()
        };

        return availablePermissionSource
            .GetQueryable(securityRule)
            .Select(bindingInfo.Principal.Path)
            .Select(visualIdentity.Name.Path)
            .Distinct()
            .GenericAsAsyncEnumerable();
    }
}