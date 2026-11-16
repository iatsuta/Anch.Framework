using Anch.Core;

namespace Anch.SecuritySystem.Testing;

/// <summary>Extensions for <see cref="ITestingEvaluator{TService}"/>.</summary>
public static class TestingEvaluatorExtensions
{
    extension<TService>(ITestingEvaluator<TService> testingEvaluator)
    {
        /// <summary>Runs code without a result as the default test user.</summary>
        /// <param name="mode">Whether the code only reads data or also changes it.</param>
        /// <param name="evaluate">The code to run.</param>
        public Task EvaluateAsync(TestingScopeMode mode, Func<TService, Task> evaluate) =>
            testingEvaluator.EvaluateAsync(mode, null, evaluate);

        /// <summary>Runs code without a result on behalf of a user.</summary>
        /// <param name="mode">Whether the code only reads data or also changes it.</param>
        /// <param name="userCredential">The user to run as; <c>null</c> for the default test user.</param>
        /// <param name="evaluate">The code to run.</param>
        public Task EvaluateAsync(TestingScopeMode mode, UserCredential? userCredential, Func<TService, Task> evaluate) =>
            testingEvaluator.EvaluateAsync(mode, userCredential, evaluate.ToDefaultTask());

        /// <summary>Runs code as the default test user.</summary>
        /// <param name="mode">Whether the code only reads data or also changes it.</param>
        /// <param name="evaluate">The code to run.</param>
        /// <typeparam name="TResult">The result type.</typeparam>
        public Task<TResult> EvaluateAsync<TResult>(TestingScopeMode mode, Func<TService, Task<TResult>> evaluate) =>
            testingEvaluator.EvaluateAsync(mode, null, evaluate);

        /// <summary>Maps the service the code receives.</summary>
        /// <param name="selector">Converts the service.</param>
        /// <typeparam name="TNewService">The new service type.</typeparam>
        public ITestingEvaluator<TNewService> Select<TNewService>(Func<TService, TNewService> selector)
        {
            return testingEvaluator.Select(async service => selector(service));
        }

        /// <summary>Maps the service the code receives, asynchronously.</summary>
        /// <param name="selector">Converts the service.</param>
        /// <typeparam name="TNewService">The new service type.</typeparam>
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