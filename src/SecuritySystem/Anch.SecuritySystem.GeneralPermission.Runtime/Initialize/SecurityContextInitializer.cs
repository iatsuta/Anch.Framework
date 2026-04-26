using Anch.Core;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.SecuritySystem.Services;
using Anch.VisualIdentitySource;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Anch.SecuritySystem.GeneralPermission.Initialize;

public class SecurityContextInitializer(IServiceProvider serviceProvider, IEnumerable<GeneralPermissionRestrictionBindingInfo> bindings)
    : ISecurityContextInitializer
{
    public async Task Initialize(CancellationToken cancellationToken)
    {
        foreach (var binding in bindings)
        {
            var initializer =
                (ISecurityContextInitializer)serviceProvider.GetRequiredService(
                    typeof(ISecurityContextInitializer<>).MakeGenericType(binding.SecurityContextTypeType));

            await initializer.Initialize(cancellationToken);
        }
    }
}

public class SecurityContextInitializer<TSecurityContextType>(
    IQueryableSource queryableSource,
    IGenericRepository genericRepository,
    ISecurityContextInfoSource securityContextInfoSource,
    ILogger<SecurityContextInitializer<TSecurityContextType>> logger,
    ISecurityIdentityManager<TSecurityContextType> securityIdentityManager,
    IVisualIdentityInfo<TSecurityContextType> visualIdentityInfo,
    InitializerSettings settings)
    : ISecurityContextInitializer<TSecurityContextType>
    where TSecurityContextType : class, new()
{
    public async Task<MergeResult<TSecurityContextType, SecurityContextInfo>> Initialize(CancellationToken cancellationToken)
    {
        var dbSecurityContextTypes = await queryableSource.GetQueryable<TSecurityContextType>().GenericToListAsync(cancellationToken);

        var mergeResult = dbSecurityContextTypes.GetMergeResult<TSecurityContextType, SecurityContextInfo, TypedSecurityIdentity>(securityContextInfoSource.SecurityContextInfoList,
            securityIdentityManager.GetIdentity,
            sc => sc.Identity);

        if (mergeResult.RemovingItems.Any())
        {
            switch (settings.UnexpectedSecurityElementMode)
            {
                case UnexpectedSecurityElementMode.RaiseError:
                    throw new InvalidOperationException(
                        $"Unexpected entity type in database: {mergeResult.RemovingItems.Join(", ")}");

                case UnexpectedSecurityElementMode.Remove:
                {
                    foreach (var removingItem in mergeResult.RemovingItems)
                    {
                        logger.LogDebug("SecurityContextType removed: {Name} {Id}", visualIdentityInfo.Name.Getter(removingItem),
                            securityIdentityManager.GetIdentity(removingItem));

                        await genericRepository.RemoveAsync(removingItem, cancellationToken);
                    }

                    break;
                }
            }
        }

        foreach (var securityContextInfo in mergeResult.AddingItems)
        {
            var securityContextType = new TSecurityContextType();

            visualIdentityInfo.Name.Setter(securityContextType, securityContextInfo.Name);
            securityIdentityManager.SetIdentity(securityContextType, securityContextInfo.Identity);

            logger.LogDebug("SecurityContextType created: {Name} {Id}", visualIdentityInfo.Name.Getter(securityContextType),
                securityIdentityManager.GetIdentity(securityContextType));

            await genericRepository.SaveAsync(securityContextType, cancellationToken);
        }

        foreach (var (securityContextType, securityContextInfo) in mergeResult.CombineItems)
        {
            if (visualIdentityInfo.Name.Getter(securityContextType) != securityContextInfo.Name)
            {
                visualIdentityInfo.Name.Setter(securityContextType, securityContextInfo.Name);

                logger.LogDebug("SecurityContextType updated: {Name} {Id}", securityContextInfo.Name, securityIdentityManager.GetIdentity(securityContextType));

                await genericRepository.SaveAsync(securityContextType, cancellationToken);
            }
        }

        return mergeResult;
    }

    async Task IInitializer.Initialize(CancellationToken cancellationToken) => await this.Initialize(cancellationToken);
}