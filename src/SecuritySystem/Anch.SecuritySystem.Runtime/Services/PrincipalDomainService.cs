using Anch.Core;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.IdentitySource;
using Anch.SecuritySystem.UserSource;
using Anch.VisualIdentitySource;

namespace Anch.SecuritySystem.Services;

public class PrincipalDomainService<TPrincipal>(
    IServiceProxyFactory serviceProxyFactory,
    IIdentityInfo<TPrincipal> identityInfo,
    IPermissionBindingInfoSource bindingInfoSource) : IPrincipalDomainService<TPrincipal>
{
    private readonly Lazy<IPrincipalDomainService<TPrincipal>> lazyInnerService = new(() =>
    {
        var bindingInfo = bindingInfoSource.GetForPrincipal(typeof(TPrincipal)).Single();

        var innerServiceType = typeof(PrincipalDomainService<,,>).MakeGenericType(
            bindingInfo.PrincipalType,
            bindingInfo.PermissionType,
            identityInfo.IdentityType);

        return serviceProxyFactory.Create<IPrincipalDomainService<TPrincipal>>(
            innerServiceType,
            bindingInfo);
    });

    private IPrincipalDomainService<TPrincipal> InnerService => this.lazyInnerService.Value;

    public Task<TPrincipal> GetOrCreateAsync(UserCredential userCredential, CancellationToken cancellationToken) =>
        this.InnerService.GetOrCreateAsync(userCredential, cancellationToken);

    public Task RemoveAsync(TPrincipal principal, bool force, CancellationToken cancellationToken) =>
        this.InnerService.RemoveAsync(principal, force, cancellationToken);
}

public class PrincipalDomainService<TPrincipal, TPermission, TPrincipalIdent>(
    PermissionBindingInfo<TPermission, TPrincipal> bindingInfo,
    IQueryableSource queryableSource,
    IGenericRepository genericRepository,
    IEnumerable<IUserSource> userSources,
    ISecurityIdentityConverter<TPrincipalIdent> identityConverter,
    IIdentityInfo<TPrincipal, TPrincipalIdent> identityInfo,
    IUserFilterFactory<TPrincipal> principalFilterFactory,
    IVisualIdentityInfo<TPrincipal> visualIdentityInfo) : IPrincipalDomainService<TPrincipal>
    where TPrincipal : class, new()
    where TPrincipalIdent : notnull
    where TPermission : class
{
    public async Task<TPrincipal> GetOrCreateAsync(UserCredential userCredential, CancellationToken cancellationToken)
    {
        var principal = await queryableSource.GetQueryable<TPrincipal>()
            .GenericSingleOrDefaultAsync(principalFilterFactory.CreateFilter(userCredential), cancellationToken);

        if (principal is null)
        {
            principal = new TPrincipal();

            switch (userCredential)
            {
                case UserCredential.FullUserCredential { User: { Name: var userName, Identity: var userIdentity } }:
                {
                    visualIdentityInfo.Name.Setter(principal, userName);

                    identityInfo.Id.Setter(principal, identityConverter.Convert(userIdentity).Id);

                    break;
                }

                case UserCredential.NamedUserCredential { Name: var userName }:
                {
                    visualIdentityInfo.Name.Setter(principal, userName);

                    if (await this.TryExtractIdent(userName, cancellationToken) is { } userIdent)
                    {
                        identityInfo.Id.Setter(principal, userIdent);
                    }

                    break;
                }

                case UserCredential.IdentUserCredential { Identity: var userIdentity }:
                {
                    if (await this.TryExtractName(userIdentity, cancellationToken) is { } userName)
                    {
                        visualIdentityInfo.Name.Setter(principal, userName);
                    }
                    else
                    {
                        throw new InvalidOperationException($"User with identity = '{userIdentity}' not found");
                    }

                    identityInfo.Id.Setter(principal, identityConverter.Convert(userIdentity).Id);
                    break;
                }
            }

            await genericRepository.SaveAsync(principal, cancellationToken);
        }

        return principal;
    }

    private async Task<TPrincipalIdent?> TryExtractIdent(string userName, CancellationToken cancellationToken)
    {
        var tryCandidates = userSources
            .ToAsyncEnumerable()
            .Where(userSource => userSource.UserType != typeof(TPrincipal))
            .Select(async (userSource, ct) => await userSource.ToSimple().TryGetUserAsync(userName, ct));

        var identRequest =

            from tryUser in tryCandidates

            where tryUser is not null

            let userIdentity = identityConverter.TryConvert(tryUser.Identity)

            where userIdentity is not null

            select userIdentity.Id;

        return await identRequest.SingleOrDefaultAsync(cancellationToken);
    }

    private async Task<string?> TryExtractName(SecurityIdentity userIdentity, CancellationToken cancellationToken)
    {
        var tryCandidates = userSources
            .ToAsyncEnumerable()
            .Where(userSource => userSource.UserType != typeof(TPrincipal))
            .Select(async (userSource, ct) => await userSource.ToSimple().TryGetUserAsync(userIdentity, ct));

        var nameRequest =

            from tryUser in tryCandidates

            where tryUser is not null

            let userName = tryUser.Name

            where userName is not null

            select userName;

        return await nameRequest.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(TPrincipal principal, bool force, CancellationToken cancellationToken)
    {
        if (!force && await queryableSource.GetQueryable<TPermission>()
                .GenericAnyAsync(bindingInfo.Principal.Path.Select(p => p == principal), cancellationToken))
        {
            throw new SecuritySystemException($"Removing principal \"{visualIdentityInfo.Name.Getter(principal)}\" must be empty");
        }

        await genericRepository.RemoveAsync(principal, cancellationToken);
    }
}