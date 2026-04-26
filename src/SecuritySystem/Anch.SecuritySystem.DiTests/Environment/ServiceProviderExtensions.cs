using Anch.SecuritySystem.Testing;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.DiTests.Environment;

public static class ServiceProviderExtensions
{
    public static void SetTestQueryable<T>(this IServiceProvider rootServiceProvider, IEnumerable<T> data)
        where T : class
    {
        var queryableSource = rootServiceProvider.GetRequiredService<TestQueryableSource>();

        queryableSource.InnerSource.GetQueryable<T>().Returns(data.AsQueryable());
    }

    public static void SetTestPermissions(this IServiceProvider rootServiceProvider,
        params TestPermission[] permissions)
    {
        var permissionStorge = rootServiceProvider.GetRequiredService<TestPermissionStorge>();

        permissionStorge.Permissions = [.. permissions];
    }
}