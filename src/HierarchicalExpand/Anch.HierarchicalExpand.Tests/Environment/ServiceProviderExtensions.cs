using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.Tests.Environment;

public static class ServiceProviderExtensions
{
    public static void SetTestQueryable<T>(this IServiceProvider rootServiceProvider, IEnumerable<T> data)
        where T : class
    {
        var queryableSource = rootServiceProvider.GetRequiredService<TestQueryableSource>();

        queryableSource.InnerSource.GetQueryable<T>().Returns(data.AsQueryable());
    }
}