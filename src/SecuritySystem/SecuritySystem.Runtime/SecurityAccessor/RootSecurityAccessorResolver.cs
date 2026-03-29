using Microsoft.Extensions.DependencyInjection;

namespace SecuritySystem.SecurityAccessor;

public class RootSecurityAccessorResolver(
    ISecurityAccessorDataOptimizer optimizer,
    [FromKeyedServices(ISecurityAccessorResolver.RawKey)]
    ISecurityAccessorResolver rawSecurityAccessorResolver) : ISecurityAccessorResolver
{
    public IEnumerable<string> Resolve(SecurityAccessorData data)
    {
        return rawSecurityAccessorResolver.Resolve(optimizer.Optimize(data));
    }
}