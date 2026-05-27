using System.Reflection;

using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTestClassRunner(IServiceProviderPool? serviceProviderPool) : XunitTestClassRunnerBase<XunitTestClassRunnerContext, IXunitTestClass, IXunitTestMethod, IXunitTestCase>
{
    protected override async ValueTask<object?> GetConstructorArgument(
        XunitTestClassRunnerContext ctxt,
        ConstructorInfo constructor,
        int index,
        ParameterInfo parameter)
    {
        if (parameter.ParameterType == typeof(IServiceProvider))
        {
            return HandledServiceProvider.Instance;
        }
        else
        {
            return await base.GetConstructorArgument(ctxt, constructor, index, parameter);
        }
    }

    protected override async ValueTask<RunSummary> RunTestMethod(XunitTestClassRunnerContext ctxt, IXunitTestMethod? testMethod,
        IReadOnlyCollection<IXunitTestCase> testCases, object?[] constructorArguments)
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

        return await new AnchTestMethodRunner(serviceProviderPool).Run(
            testMethod,
            testCases,
            ctxt.ExplicitOption,
            ctxt.MessageBus,
            ctxt.Aggregator.Clone(),
            ctxt.CancellationTokenSource,
            constructorArguments);
    }

    public async ValueTask<RunSummary> Run(
        IXunitTestClass testClass,
        IReadOnlyCollection<IXunitTestCase> testCases,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        ITestCaseOrderer testCaseOrderer,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        FixtureMappingManager assemblyFixtureMappings)
    {
        Guard.ArgumentNotNull(testClass);
        Guard.ArgumentNotNull(testCases);
        Guard.ArgumentNotNull(messageBus);
        Guard.ArgumentNotNull(testCaseOrderer);
        Guard.ArgumentNotNull(cancellationTokenSource);
        Guard.ArgumentNotNull(assemblyFixtureMappings);

        await using var ctxt = new XunitTestClassRunnerContext(
            testClass,
            testCases,
            explicitOption,
            messageBus,
            testCaseOrderer,
            aggregator,
            cancellationTokenSource,
            assemblyFixtureMappings);

        await ctxt.InitializeAsync();

        return await this.Run(ctxt);
    }
}