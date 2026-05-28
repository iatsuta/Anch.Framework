using System.Globalization;

using Xunit;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTestRunner(IServiceProviderPool? serviceProviderPool) : XunitTestRunnerBase<XunitTestRunnerContext, IXunitTest>
{
    public async ValueTask<RunSummary> Run(
        IXunitTest test,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExplicitOption explicitOption,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterAttributes)
    {
        await using var serviceProviderPoolScope = await serviceProviderPool.TryCreateScopeAsync(cancellationTokenSource.Token);

        if (serviceProviderPoolScope?.Exception == null)
        {
            await using var ctxt = new XunitTestRunnerContext(
                test,
                messageBus,
                explicitOption,
                aggregator,
                cancellationTokenSource,
                beforeAfterAttributes,
                constructorArguments.Select(arg => arg == HandledServiceProvider.Instance ? serviceProviderPoolScope?.ServiceProvider : arg).ToArray()
            );

            await ctxt.InitializeAsync();

            return await this.Run(ctxt);
        }
        else
        {
            return XunitRunnerHelper.FailTest(messageBus, cancellationTokenSource, test, serviceProviderPoolScope.Exception);
        }
    }

    protected override object? InvokeTestMethod(XunitTestRunnerContext ctxt, object? testClassInstance)
    {
        if (ctxt.TestMethod.LastParameterIsCt())
        {
            return Guard.ArgumentNotNull(ctxt).TestMethod.Invoke(testClassInstance, [.. ctxt.TestMethodArguments, TestContext.Current.CancellationToken]);
        }
        else
        {
            return base.InvokeTestMethod(ctxt, testClassInstance);
        }
    }

    protected override ValueTask<TimeSpan> InvokeTest(
        XunitTestRunnerContext ctxt,
        object? testClassInstance)
    {
        if (!ctxt.TestMethod.LastParameterIsCt())
        {
            return base.InvokeTest(ctxt, testClassInstance);
        }

        Guard.ArgumentNotNull(ctxt);

        if (ctxt.Test.TestCase.TestMethod is null)
        {
            ctxt.Aggregator.Add(
                new TestPipelineException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Test '{0}' does not have an associated method and cannot be run by TestRunner",
                        ctxt.Test.TestDisplayName
                    )
                )
            );

            return new(TimeSpan.Zero);
        }

        return ExecutionTimer.MeasureAsync(
            () => ctxt.Aggregator.RunAsync(
                async () =>
                {
                    var parameterCount = ctxt.TestMethod.GetParameters().Length;
                    var valueCount = ctxt.TestMethodArguments is null ? 0 : ctxt.TestMethodArguments.Length + 1;
                    if (parameterCount != valueCount)
                    {
                        ctxt.Aggregator.Add(
                            new InvalidOperationException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    "The test method expected {0} parameter value{1}, but {2} parameter value{3} {4} provided.",
                                    parameterCount,
                                    parameterCount == 1 ? "" : "s",
                                    valueCount,
                                    valueCount == 1 ? "" : "s",
                                    valueCount == 1 ? "was" : "were"
                                )
                            )
                        );
                    }
                    else
                    {
                        var result = this.InvokeTestMethod(ctxt, testClassInstance);
                        var valueTask = AsyncUtility.TryConvertToValueTask(result);
                        if (valueTask.HasValue)
                            await valueTask.Value;
                    }
                }
            )
        );
    }
}