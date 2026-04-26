namespace Anch.SecuritySystem.Providers;

/// <summary>
/// Провайдер доступа с фиксированным ответом для одного типа
/// </summary>
/// <typeparam name="TDomainObject"></typeparam>
public class ConstSecurityProvider<TDomainObject>(bool hasAccess) : ISecurityProvider<TDomainObject>
{
    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable)
    {
        return queryable.Where(_ => hasAccess);
    }

    public async ValueTask<bool> HasAccess(TDomainObject _, CancellationToken cancellationToken) => hasAccess;
}