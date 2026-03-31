using CommonFramework;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource;
using CommonFramework.VisualIdentitySource;

using GenericQueryable;

using HierarchicalExpand;
using HierarchicalExpand.Denormalization;

using SecuritySystem.Credential;
using SecuritySystem.DomainServices;

using Microsoft.Extensions.DependencyInjection;

namespace SecuritySystem.Testing;

public class RootAuthManager(
    IServiceProvider rootServiceProvider,
    IServiceProxyFactory serviceProxyFactory,
    ITestingEvaluator<IQueryableSource> queryableSourceEvaluator,
    ITestingEvaluator<IServiceProvider> serviceProviderEvaluator,
    IIdentityInfoSource identityInfoSource,
    IVisualIdentityInfoSource visualIdentityInfoSource,
    TestRootUserInfo rootUserInfo)
{
    public string RootUserName => rootUserInfo.Name;

    public RootUserCredentialManager For(UserCredential? userCredential = null)
    {
        return serviceProxyFactory.Create<RootUserCredentialManager>(Tuple.Create(userCredential));
    }

    public async Task<List<TypedSecurityIdentity<TIdent>>> GetIdentityListAsync<TDomainObject, TIdent>(SecurityRule securityRule,
        CancellationToken cancellationToken)
        where TIdent : notnull
        where TDomainObject : class
    {
        var identityInfo = identityInfoSource.GetIdentityInfo<TDomainObject, TIdent>();

        return await serviceProviderEvaluator.EvaluateAsync(TestingScopeMode.Read, async serviceProvider =>
        {
            var securityProvider = serviceProvider.GetRequiredService<IDomainSecurityService<TDomainObject>>().GetSecurityProvider(securityRule);

            var queryableSource = serviceProvider.GetRequiredService<IQueryableSource>();

            var idents = await queryableSource.GetQueryable<TDomainObject>().Pipe(securityProvider.Inject).Select(identityInfo.Id.Path)
                .GenericToListAsync(cancellationToken);

            return idents.Select(TypedSecurityIdentity.Create).ToList();
        });
    }

    public async Task<TypedSecurityIdentity<TIdent>> GetSecurityContextIdentityAsync<TSecurityContext, TIdent>(string name, CancellationToken cancellationToken)
        where TSecurityContext : class, ISecurityContext
        where TIdent : notnull
    {
        var identityInfo = identityInfoSource.GetIdentityInfo<TSecurityContext, TIdent>();
        var visualIdentityInfo = visualIdentityInfoSource.GetVisualIdentityInfo<TSecurityContext>();

        var filter = visualIdentityInfo.Name.Path.Select(v => v == name);

        return await queryableSourceEvaluator.EvaluateAsync(TestingScopeMode.Read, async queryableSource =>
        {
            var securityContextId = await queryableSource.GetQueryable<TSecurityContext>().Where(filter).Select(identityInfo.Id.Path)
                .GenericSingleAsync(cancellationToken);

            return TypedSecurityIdentity.Create(securityContextId);
        });
    }

    public Task<TypedSecurityIdentity<TIdent>> SaveSecurityContextAsync<TSecurityContext, TIdent>(
        Func<TSecurityContext> createFunc,
        CancellationToken cancellationToken)
        where TSecurityContext : class, ISecurityContext
        where TIdent : notnull => this.SaveSecurityContextAsync<TSecurityContext, TIdent>(async _ => createFunc(), cancellationToken);

    public async Task<TypedSecurityIdentity<TIdent>> SaveSecurityContextAsync<TSecurityContext, TIdent>(Func<IServiceProvider, Task<TSecurityContext>> createFunc,
        CancellationToken cancellationToken)
        where TSecurityContext : class, ISecurityContext
        where TIdent : notnull
    {
        var identityInfo = identityInfoSource.GetIdentityInfo<TSecurityContext, TIdent>();

        var id = await serviceProviderEvaluator.EvaluateAsync(TestingScopeMode.Write, async sp =>
        {
            var securityContext = await createFunc(sp);

            var genericRepository = sp.GetRequiredService<IGenericRepository>();

            await genericRepository.SaveAsync(securityContext, cancellationToken);

            return identityInfo.Id.Getter(securityContext);
        });

        if (rootServiceProvider.GetService(typeof(FullAncestorLinkInfo<TSecurityContext>)) != null)
        {
            await serviceProviderEvaluator.EvaluateAsync(TestingScopeMode.Write, async serviceProvider =>
            {
                var queryableSource = serviceProvider.GetRequiredService<IQueryableSource>();

                var securityContext = await queryableSource.GetQueryable<TSecurityContext>()
                    .Where(identityInfo.Id.Path.Select(ExpressionHelper.GetEqualityWithExpr(id))).GenericSingleAsync(cancellationToken);

                {
                    var ancestorDenormalizer = serviceProvider.GetRequiredService<IAncestorDenormalizer<TSecurityContext>>();

                    await ancestorDenormalizer.SyncAsync([securityContext], [], cancellationToken);
                }

                if (rootServiceProvider.GetService(typeof(DeepLevelInfo<TSecurityContext>)) != null)
                {
                    var deepLevelDenormalizer = serviceProvider.GetRequiredService<IDeepLevelDenormalizer<TSecurityContext>>();

                    await deepLevelDenormalizer.UpdateDeepLevels([securityContext], cancellationToken);
                }
            });
        }

        return TypedSecurityIdentity.Create(id);
    }
}