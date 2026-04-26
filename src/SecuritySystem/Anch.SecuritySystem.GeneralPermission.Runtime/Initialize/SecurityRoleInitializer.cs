using Anch.Core;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.SecuritySystem.Services;
using Anch.VisualIdentitySource;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Anch.SecuritySystem.GeneralPermission.Initialize;

public class SecurityRoleInitializer(IServiceProvider serviceProvider, IEnumerable<GeneralPermissionBindingInfo> bindings)
    : ISecurityRoleInitializer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        foreach (var binding in bindings)
        {
            var initializer =
                (ISecurityRoleInitializer)serviceProvider.GetRequiredService(
                    typeof(ISecurityRoleInitializer<>).MakeGenericType(binding.SecurityRoleType));

            await initializer.Initialize(cancellationToken);
        }
    }
}

public class SecurityRoleInitializer<TPermission, TSecurityRole>(
    GeneralPermissionBindingInfo<TPermission, TSecurityRole> bindingInfo,
    IQueryableSource queryableSource,
    IGenericRepository genericRepository,
    ISecurityRoleSource securityRoleSource,
    ILogger<SecurityRoleInitializer<TPermission, TSecurityRole>> logger,
    IVisualIdentityInfo<TSecurityRole> visualIdentityInfo,
    ISecurityIdentityManager<TSecurityRole> securityIdentityManager,
    InitializerSettings settings)
    : ISecurityRoleInitializer<TSecurityRole>
    where TSecurityRole : class, new()
{
    public async Task<MergeResult<TSecurityRole, FullSecurityRole>> Initialize(CancellationToken cancellationToken)
    {
        return await this.Initialize(securityRoleSource.GetRealRoles(), cancellationToken);
    }

    public async Task<MergeResult<TSecurityRole, FullSecurityRole>> Initialize(
        IEnumerable<FullSecurityRole> securityRoles,
        CancellationToken cancellationToken)
    {
        var dbRoles = await queryableSource.GetQueryable<TSecurityRole>().GenericToListAsync(cancellationToken);

        var mergeResult = dbRoles.GetMergeResult<TSecurityRole, FullSecurityRole, TypedSecurityIdentity>(securityRoles, securityIdentityManager.GetIdentity, fsr => fsr.Identity);

        if (mergeResult.RemovingItems.Any())
        {
            switch (settings.UnexpectedSecurityElementMode)
            {
                case UnexpectedSecurityElementMode.RaiseError:
                    throw new InvalidOperationException(
                        $"Unexpected roles in database: {mergeResult.RemovingItems.Join(", ")}");

                case UnexpectedSecurityElementMode.Remove:
                {
                    foreach (var removingItem in mergeResult.RemovingItems)
                    {
                        logger.LogDebug("Role removed: {Name} {Id}", visualIdentityInfo.Name.Getter(removingItem), securityIdentityManager.GetIdentity(removingItem));

                        await genericRepository.RemoveAsync(removingItem, cancellationToken);
                    }

                    break;
                }
            }
        }

        foreach (var securityRole in mergeResult.AddingItems)
        {
            var dbSecurityRole = new TSecurityRole();

            visualIdentityInfo.Name.Setter(dbSecurityRole, securityRole.Name);
            bindingInfo.SecurityRoleDescription?.Setter(dbSecurityRole, securityRole.Information.Description ?? "");
            securityIdentityManager.SetIdentity(dbSecurityRole, securityRole.Identity);

            logger.LogDebug("Role created: {Name} {Id}", securityRole.Name, securityRole.Identity);

            await genericRepository.SaveAsync(dbSecurityRole, cancellationToken);
        }

        foreach (var (dbSecurityRole, securityRole) in mergeResult.CombineItems)
        {
            var newName = securityRole.Name;
            var newDescription = securityRole.Information.Description ?? "";

            if (newName != visualIdentityInfo.Name.Getter(dbSecurityRole) || (bindingInfo.SecurityRoleDescription != null &&
                                                                              newDescription != bindingInfo.SecurityRoleDescription.Getter(dbSecurityRole)))
            {
                visualIdentityInfo.Name.Setter(dbSecurityRole, newName);
                bindingInfo.SecurityRoleDescription?.Setter(dbSecurityRole, newDescription);

                logger.LogDebug("Role updated: {Name} {Description} {Id}", newName, newDescription, securityIdentityManager.GetIdentity(dbSecurityRole));

                await genericRepository.SaveAsync(dbSecurityRole, cancellationToken);
            }
        }

        return mergeResult;
    }

    async Task IInitializer.Initialize(CancellationToken cancellationToken) => await this.Initialize(cancellationToken);
}