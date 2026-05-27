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

        await using var scope = await serviceProviderPool.CreateScopeAsync(ctxt.CancellationTokenSource.Token);

        if (scope.Exception != null)
        {
            return XunitRunnerHelper.FailTestCases(
                ctxt.MessageBus,
                ctxt.CancellationTokenSource,
                [testCase],
                scope.Exception,
                sendTestClassMessages: true
            );
        }

        if (testCase is ISelfExecutingXunitTestCase selfExecutingTestCase)
            return await selfExecutingTestCase.Run(ctxt.ExplicitOption, ctxt.MessageBus, ctxt.ConstructorArguments, ctxt.Aggregator.Clone(),
                ctxt.CancellationTokenSource);

        return await AnchRunnerHelper.RunXunitTestCase(
            testCase,
            ctxt.MessageBus,
            ctxt.CancellationTokenSource,
            ctxt.Aggregator.Clone(),
            ctxt.ExplicitOption,
            ctxt.ConstructorArguments.Select(arg => arg == HandledServiceProvider.Instance ? scope.ServiceProvider : arg).ToArray()
        );
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