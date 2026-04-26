using Xunit.Internal;
using Xunit.v3;

namespace Anch.Testing.XunitEngine;

public class AnchTestCollectionRunner(AnchTestClassRunner commonTestClassRunner, IServiceProviderPool? serviceProviderPool) :
    XunitTestCollectionRunner
{
    protected override async ValueTask<RunSummary> RunTestClass(XunitTestCollectionRunnerContext ctxt, IXunitTestClass? testClass, IReadOnlyCollection<IXunitTestCase> testCases)
    {
        Guard.ArgumentNotNull(ctxt);

        // Technically not possible because of the design of TTestClass, but this signature is imposed
        // by the base class, which allows class-less tests
        if (testClass is null)
            return XunitRunnerHelper.FailTestCases(
                ctxt.MessageBus,
                ctxt.CancellationTokenSource,
                testCases,
                "Test case '{0}' does not have an associated class and cannot be run by XunitTestClassRunner",
                sendTestClassMessages: true
            );

        var serviceProvider = serviceProviderPool?.Get();

        try
        {
            return await commonTestClassRunner.Run(
                testClass,
                testCases,
                ctxt.ExplicitOption,
                ctxt.MessageBus,
                ctxt.TestCaseOrderer,
                ctxt.Aggregator.Clone(),
                ctxt.CancellationTokenSource,
                ctxt.CollectionFixtureMappings,
                serviceProvider);
        }
        finally
        {
            if (serviceProviderPool != null && serviceProvider is not null)
                serviceProviderPool.Release(serviceProvider);
        }
    }
}