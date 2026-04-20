using System.Collections.ObjectModel;
using System.Reflection;

using Xunit;
using Xunit.Internal;
using Xunit.v3;

namespace CommonFramework.Testing;

public class CommonTestClassRunner(IServiceProvider rootServiceProvider) : XunitTestClassRunner
{
    protected override ValueTask<object?> GetConstructorArgument(
        XunitTestClassRunnerContext ctxt,
        ConstructorInfo constructor,
        int index,
        ParameterInfo parameter)
    {
        if (parameter.ParameterType == typeof(IServiceProvider))
        {
            return new ValueTask<object?>(rootServiceProvider);
        }
        else
        {
            return base.GetConstructorArgument(ctxt, constructor, index, parameter);
        }
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

        return await new CommonTestMethodRunner(rootServiceProvider).Run(
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

public class CommonTestMethodRunner(IServiceProvider rootServiceProvider) : XunitTestMethodRunner
{
    protected override ValueTask<RunSummary> RunTestCase(XunitTestMethodRunnerContext ctxt, IXunitTestCase testCase)
    {
        // if (testCase is CommonXunitTestCase commonXunitTestCase)
        // {
        ////     commonXunitTestCase.TestMethodArguments[^1] = TestContext.Current.CancellationToken;
        // }

        // return base.RunTestCase(ctxt, testCase);

        Guard.ArgumentNotNull(ctxt);
        Guard.ArgumentNotNull(testCase);

        if (testCase is ISelfExecutingXunitTestCase selfExecutingTestCase)
            return selfExecutingTestCase.Run(ctxt.ExplicitOption, ctxt.MessageBus, ctxt.ConstructorArguments, ctxt.Aggregator.Clone(), ctxt.CancellationTokenSource);

        return CommonRunnerHelper.RunXunitTestCase(
            testCase,
            ctxt.MessageBus,
            ctxt.CancellationTokenSource,
            ctxt.Aggregator.Clone(),
            ctxt.ExplicitOption,
            ctxt.ConstructorArguments
        );
    }
}