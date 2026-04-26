using Anch.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public class ScopeEvaluator(IServiceProvider rootServiceProvider)
{
    public Task EvaluateAsync(Func<IServiceProvider, Task> action) => this.EvaluateAsync<IServiceProvider>(action);

    public Task EvaluateAsync<TService>(Func<TService, Task> action)
        where TService : notnull => this.EvaluateAsync(action.ToDefaultTask());

    public Task<TResult> EvaluateAsync<TResult>(Func<IServiceProvider, Task<TResult>> func) => this.EvaluateAsync<IServiceProvider, TResult>(func);

    public async Task<TResult> EvaluateAsync<TService, TResult>(Func<TService, Task<TResult>> func)
        where TService : notnull
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        return await func(scope.ServiceProvider.GetRequiredService<TService>());
    }
}