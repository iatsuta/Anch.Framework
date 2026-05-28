using System.Diagnostics;

using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTestMethodRunner(IServiceProviderPool? serviceProviderPool)
    : XunitTestMethodRunnerBase<XunitTestMethodRunnerContext, IXunitTestMethod, IXunitTestCase>
{
    protected override async ValueTask<RunSummary> RunTestCase(XunitTestMethodRunnerContext ctxt, IXunitTestCase testCase)
    {
        Guard.ArgumentNotNull(ctxt);
        Guard.ArgumentNotNull(testCase);

        if (testCase is ISelfExecutingXunitTestCase selfExecutingTestCase)
        {
            return await selfExecutingTestCase.Run(ctxt.ExplicitOption, ctxt.MessageBus, ctxt.ConstructorArguments, ctxt.Aggregator.Clone(),
                ctxt.CancellationTokenSource);
        }
        else
        {
            var actualTestCase = testCase.WithServiceProviderPool(serviceProviderPool);

            return await AnchRunnerHelper.RunXunitTestCase(
                actualTestCase,
                ctxt.MessageBus,
                ctxt.CancellationTokenSource,
                ctxt.Aggregator.Clone(),
                ctxt.ExplicitOption,
                ctxt.ConstructorArguments,
                serviceProviderPool);
        }
    }

    public async ValueTask<RunSummary> Run(
        IXunitTestMethod testMethod,
        IReadOnlyCollection<IXunitTestCase> testCases,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        object?[] constructorArguments)
    {
        Guard.ArgumentNotNull(testCases);
        Guard.ArgumentNotNull(messageBus);
        Guard.ArgumentNotNull(constructorArguments);

        await using var ctxt = new XunitTestMethodRunnerContext(
            testMethod,
            testCases,
            explicitOption,
            messageBus,
            aggregator,
            cancellationTokenSource,
            constructorArguments);

        await ctxt.InitializeAsync();

        return await this.Run(ctxt);
    }
}