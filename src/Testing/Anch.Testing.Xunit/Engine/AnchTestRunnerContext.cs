using System.Globalization;

using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTestRunnerContext(
    IXunitTest test,
    ExplicitOption explicitOption,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    ParallelMode parallelMode,
    ExecutionScheduler scheduler,
    IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterTestAttributes,
    object?[] constructorArguments,
    FixtureMappingManager caseFixtureMappings)
    : XunitTestRunnerContext(
        test,
        explicitOption,
        messageBus,
        aggregator,
        cancellationTokenSource,
        parallelMode,
        scheduler,
        beforeAfterTestAttributes,
        constructorArguments,
        caseFixtureMappings)
{
    protected override object? InvokeTestMethod(object? testClassInstance)
    {
        if (this.Method.LastParameterIsCt())
        {
            return this.Method.Invoke(testClassInstance, [.. this.MethodArguments, TestContext.Current.CancellationToken]);
        }
        else
        {
            return base.InvokeTestMethod(testClassInstance);
        }
    }

    public override ValueTask<TimeSpan> InvokeTest(object? testClassInstance)
    {
        if (!this.Method.LastParameterIsCt())
        {
            return base.InvokeTest(testClassInstance);
        }

        if (this.Test.TestCase.TestMethod is null)
        {
            this.Aggregator.Add(
                new TestPipelineException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Test '{0}' does not have an associated method and cannot be run by TestRunner",
                        this.Test.TestDisplayName
                    )
                )
            );

            return new(TimeSpan.Zero);
        }

        return ExecutionTimer.MeasureAsync(
            () => this.Aggregator.RunAsync(
                async () =>
                {
                    var parameterCount = this.Method.GetParameters().Length;
                    var valueCount = this.MethodArguments is null ? 0 : this.MethodArguments.Length + 1;
                    if (parameterCount != valueCount)
                    {
                        this.Aggregator.Add(
                            new InvalidOperationException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
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
                        var result = this.InvokeTestMethod(testClassInstance);
                        var valueTask = AsyncUtility.TryConvertToValueTask(result);
                        if (valueTask.HasValue)
                            await valueTask.Value;
                    }
                }
            )
        );
    }
}
