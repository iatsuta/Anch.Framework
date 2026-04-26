using Anch.SecuritySystem.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Testing;

public class TestingEvaluator<TService>(IServiceProvider rootServiceProvider) : ITestingEvaluator<TService>
    where TService : notnull
{
    public async Task<TResult> EvaluateAsync<TResult>(TestingScopeMode mode, UserCredential? userCredential, Func<TService, Task<TResult>> evaluate)
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        if (userCredential == null)
        {
            var service = scope.ServiceProvider.GetRequiredService<TService>();

            return await evaluate(service);
        }
        else
        {
            var impersonateService = scope.ServiceProvider.GetRequiredService<IImpersonateService>();

            return await impersonateService.WithImpersonateAsync(userCredential, async () =>
            {
                var service = scope.ServiceProvider.GetRequiredService<TService>();

                return await evaluate(service);
            });
        }
    }
}