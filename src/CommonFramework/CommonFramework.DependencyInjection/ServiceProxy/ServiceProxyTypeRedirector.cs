using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace CommonFramework.DependencyInjection.ServiceProxy;

public class ServiceProxyTypeRedirector(IEnumerable<ServiceProxyTypeRedirectInfo> infoList, INativeActivator nativeActivator) : IServiceProxyTypeRedirector
{
    private readonly ImmutableArray<ServiceProxyTypeRedirectInfo> cachedList = [..infoList];

    private readonly ConcurrentDictionary<Type, Type?> cache = [];

    public Type? TryRedirect(Type sourceType) => this.cache.GetOrAdd(sourceType, _ => this.TryFindRedirectInfo(sourceType).Maybe(this.GetTargetType));

    private Type GetTargetType(ServiceProxyTypeRedirectInfo redirectInfo) =>
        redirectInfo.IsBinder ? nativeActivator.Create<IServiceProxyBinder>(redirectInfo.TargetType).GetTargetServiceType() : redirectInfo.TargetType;

    private ServiceProxyTypeRedirectInfo? TryFindRedirectInfo(Type sourceType) =>
        this.GetRedirectCandidates(sourceType)
            .Aggregate(default(ServiceProxyTypeRedirectInfo?), (prev, next) =>
            {
                if (prev == null)
                {
                    return next.Replace ? throw new InvalidOperationException("The first candidate cannot perform a replacement") : next;
                }
                else
                {
                    return next.Replace ? next : throw new InvalidOperationException("Each subsequent candidate must replace the previous candidate");
                }
            });

    private IEnumerable<ServiceProxyTypeRedirectInfo> GetRedirectCandidates(Type sourceType)
    {
        foreach (var redirectInfo in this.cachedList)
        {
            if (redirectInfo.SourceType == sourceType)
            {
                yield return redirectInfo;
            }
            else if (redirectInfo.TargetType.IsGenericTypeDefinition && sourceType.IsGenericTypeImplementation(redirectInfo.SourceType))
            {
                var newTargetType = redirectInfo.TargetType.MakeGenericType(sourceType.GetGenericArguments());

                yield return redirectInfo with { SourceType = sourceType, TargetType = newTargetType };
            }
        }
    }
}