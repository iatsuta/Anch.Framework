using System.Collections.Concurrent;
using System.Reflection;

using Xunit.Internal;
using Xunit.v3;

namespace CommonFramework.Testing.XunitEngine;

public class CommonTestClassRunner(IServiceProviderPool? rootServiceProvider) : XunitTestClassRunner
{
    private readonly ConcurrentDictionary<XunitTestClassRunnerContext, Task<IServiceProvider>> serviceProviderCache = [];

    protected override async ValueTask<object?> GetConstructorArgument(
        XunitTestClassRunnerContext ctxt,
        ConstructorInfo constructor,
        int index,
        ParameterInfo parameter)
    {
        if (rootServiceProvider != null && parameter.ParameterType == typeof(IServiceProvider))
        {
            return await this.serviceProviderCache.GetOrAdd(ctxt, async _ => await rootServiceProvider.GetAsync(ctxt.CancellationTokenSource.Token));
        }
        else
        {
            return await base.GetConstructorArgument(ctxt, constructor, index, parameter);
        }
    }

    protected override async ValueTask<bool> OnTestClassFinished(XunitTestClassRunnerContext ctxt, RunSummary summary)
    {
        var result = await base.OnTestClassFinished(ctxt, summary);

        if (rootServiceProvider != null && this.serviceProviderCache.Remove(ctxt, out var getSpTask))
        {
            await rootServiceProvider.ReleaseAsync(await getSpTask, ctxt.CancellationTokenSource.Token);
        }

        return result;
    }

    protected override async ValueTask<RunSummary> RunTestMethod(XunitTestClassRunnerContext ctxt, IXunitTestMethod? testMethod, IReadOnlyCollection<IXunitTestCase> testCases, object?[] constructorArguments)
    {
        Guard.ArgumentNotNull(ctxt);

        // Technically not possible because of the design of TTestClass, but this signature is imposed
        // by the base class, which allows method-less tests
        if (testMethod is null)
            return XunitRunnerHelper.FailTestCases(
                ctxt.MessageBus,
                ctxt.CancellationTokenSource,
                testCases,
                "Test case '{0}' does not have an associated method and cannot be run by XunitTestMethodRunner",
                sendTestMethodMessages: true
            );

        //await ExecutionTimer.MeasureAsync(null);

        return await CommonTestMethodRunner.Instance.Run(
            testMethod,
            testCases,
            ctxt.ExplicitOption,
            ctxt.MessageBus,
            ctxt.Aggregator.Clone(),
            ctxt.CancellationTokenSource,
            constructorArguments
        );
    }
}