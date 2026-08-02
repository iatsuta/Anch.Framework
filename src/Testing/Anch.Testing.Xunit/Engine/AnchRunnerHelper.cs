using System.Globalization;

using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public static class AnchRunnerHelper
{
    /// <summary>
    /// Runs a single test case (which implements <see cref="IXunitTestCase"/>) using
    /// the <see cref="XunitTestCaseRunner"/> after enumerating all tests.
    /// </summary>
    /// <param name="testCase">The test case to run</param>
    /// <param name="messageBus">The message bus to send the messages to</param>
    /// <param name="cancellationTokenSource">The cancellation token source to cancel if requested</param>
    /// <param name="aggregator">The exception aggregator to record exceptions to</param>
    /// <param name="explicitOption">A flag to indicate which types of tests to run (non-explicit, explicit, or both)</param>
    /// <param name="constructorArguments">The arguments to pass to the test class constructor</param>
    /// <returns></returns>
    public static async ValueTask<RunSummary> RunXunitTestCase(
        IXunitTestCase testCase,
        IMessageBus messageBus,
        CancellationTokenSource cancellationTokenSource,
        ExceptionAggregator aggregator,
        ExplicitOption explicitOption,
        object?[] constructorArguments,
        IServiceProviderPool? serviceProviderPool)
    {
        Guard.ArgumentNotNull(testCase);

        var tests = await aggregator.RunAsync(testCase.CreateTests, []);

        if (aggregator.ToException() is  Exception ex)
        {
            if (ex.Message?.StartsWith(DynamicSkipToken.Value, StringComparison.Ordinal) == true)
                return XunitRunnerHelper.SkipTestCases(
                    messageBus,
                    cancellationTokenSource,
                    [testCase],
                    ex.Message.Substring(DynamicSkipToken.Value.Length),
                    sendTestCaseMessages: false
                );
            else if (testCase.SkipExceptions?.Contains(ex.GetType()) == true)
                return XunitRunnerHelper.SkipTestCases(
                    messageBus,
                    cancellationTokenSource,
                    [testCase],
                    ex.Message is not null && ex.Message.Length != 0
                        ? ex.Message
                        : string.Format(CultureInfo.CurrentCulture, "Exception of type '{0}' was thrown", ex.GetType().SafeName()),
                    sendTestCaseMessages: false
                );
            else
                return XunitRunnerHelper.FailTestCases(
                    messageBus,
                    cancellationTokenSource,
                    [testCase],
                    ex,
                    sendTestCaseMessages: false
                );
        }

        return await new AnchTestCaseRunner(serviceProviderPool).Run(
            testCase,
            tests,
            messageBus,
            aggregator,
            cancellationTokenSource,
            testCase.TestCaseDisplayName,
            testCase.SkipReason,
            explicitOption,
            constructorArguments
        );
    }
}