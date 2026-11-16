namespace Anch.SecuritySystem.Providers;

/// <summary>
/// Security provider with a fixed answer for a single type.
/// </summary>
/// <typeparam name="TDomainObject"></typeparam>
public class ConstSecurityProvider<TDomainObject>(bool hasAccess) : ISecurityProvider<TDomainObject>
{
    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable)
    {
        return queryable.Where(_ => hasAccess);
    }

    public ValueTask<bool> HasAccessAsync(TDomainObject _, CancellationToken __) => new(hasAccess);
}