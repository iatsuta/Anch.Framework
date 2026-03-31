using CommonFramework;
using CommonFramework.ExpressionEvaluate;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource;

using GenericQueryable;

using SecuritySystem.SecurityAccessor;

namespace SecuritySystem.Providers.DependencySecurity;

public class UntypedDependencySecurityProvider<TDomainObject, TBaseDomainObject, TIdent>(
    IQueryableSource queryableSource,
    ISecurityProvider<TBaseDomainObject> baseSecurityProvider,
    IIdentityInfo<TDomainObject, TIdent> domainIdentityInfo,
    IIdentityInfo<TBaseDomainObject, TIdent> baseDomainIdentityInfo)
    : ISecurityProvider<TDomainObject>
    where TBaseDomainObject : class
    where TIdent : notnull
{
    private HashSet<TIdent>? availableIdentsCache;

    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable)
    {
        var availableIdentsQ = this.GetAvailableIdentsQ();

        var filterExpr = ExpressionEvaluateHelper.InlineEvaluate<Func<TDomainObject, bool>>(ee =>

            domainObject => availableIdentsQ.Contains(ee.Evaluate(domainIdentityInfo.Id.Path, domainObject)));

        return queryable.Where(filterExpr);
    }

    public async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        return (await baseSecurityProvider.GetAccessResultAsync(this.GetBaseObject(domainObject), cancellationToken)).TryOverrideDomainObject(domainObject);
    }

    public async ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        return (this.availableIdentsCache ??= await this.GetAvailableIdentsQ().GenericToHashSetAsync(cancellationToken))
            .Contains(domainIdentityInfo.Id.Getter(domainObject));
    }

    public ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        return baseSecurityProvider.GetAccessorDataAsync(this.GetBaseObject(domainObject), cancellationToken);
    }

    private TBaseDomainObject GetBaseObject(TDomainObject domainObject)
    {
        var id = domainIdentityInfo.Id.Getter(domainObject);

        var eqIdExp = ExpressionHelper.GetEquality<TIdent>();

        var filterExpr = ExpressionEvaluateHelper.InlineEvaluate<Func<TBaseDomainObject, bool>>(ee =>

            baseDomainObject => ee.Evaluate(eqIdExp, ee.Evaluate(baseDomainIdentityInfo.Id.Path, baseDomainObject), id));

        return queryableSource
                   .GetQueryable<TBaseDomainObject>()
                   .SingleOrDefault(filterExpr)
                   .FromMaybe(() => $"{typeof(TBaseDomainObject).Name} with id = '{id}' not found");
    }

    protected virtual IQueryable<TIdent> GetAvailableIdentsQ()
    {
        return queryableSource.GetQueryable<TBaseDomainObject>().Pipe(baseSecurityProvider.Inject).Select(baseDomainIdentityInfo.Id.Path);
    }
}
