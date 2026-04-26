using Anch.Core;

namespace Anch.SecuritySystem.Testing;

public static class TestingEvaluatorExtensions
{
    extension<TService>(ITestingEvaluator<TService> testingEvaluator)
    {
        public Task EvaluateAsync(TestingScopeMode mode, Func<TService, Task> evaluate) =>
            testingEvaluator.EvaluateAsync(mode, null, evaluate);

        public Task EvaluateAsync(TestingScopeMode mode, UserCredential? userCredential, Func<TService, Task> evaluate) =>
            testingEvaluator.EvaluateAsync(mode, userCredential, evaluate.ToDefaultTask());

        public Task<TResult> EvaluateAsync<TResult>(TestingScopeMode mode, Func<TService, Task<TResult>> evaluate) =>
            testingEvaluator.EvaluateAsync(mode, null, evaluate);

        public ITestingEvaluator<TNewService> Select<TNewService>(Func<TService, TNewService> selector)
        {
            return testingEvaluator.Select(async service => selector(service));
        }

        public ITestingEvaluator<TNewService> Select<TNewService>(Func<TService, Task<TNewService>> selector)
        {
            return new ChangedTestingEvaluator<TService, TNewService>(testingEvaluator, selector);
        }
    }

    private class ChangedTestingEvaluator<TService, TNewService>(ITestingEvaluator<TService> baseEvaluator, Func<TService, Task<TNewService>> selector)
        : ITestingEvaluator<TNewService>
    {
        public async Task<TResult> EvaluateAsync<TResult>(TestingScopeMode mode, UserCredential? userUserCredential,
            Func<TNewService, Task<TResult>> evaluate) =>

            await baseEvaluator.EvaluateAsync(mode, async baseService =>
            {
                var newService = await selector(baseService);

                return await evaluate(newService);
            });
    }
}