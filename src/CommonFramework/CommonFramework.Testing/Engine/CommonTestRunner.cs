using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Xunit;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace CommonFramework.Testing.Engine;

public class CommonTestRunner : XunitTestRunner
{
    public new static CommonTestRunner Instance { get; } = new();

    private static readonly ConcurrentDictionary<MethodInfo, bool> IsCtCache = [];

    private bool LastParameterIsCt(XunitTestRunnerContext ctxt)
    {
        return IsCtCache.GetOrAdd(ctxt.TestMethod, m => m.GetParameters().LastOrDefault()?.ParameterType == typeof(CancellationToken));
    }

    protected override object? InvokeTestMethod(XunitTestRunnerContext ctxt, object? testClassInstance)
    {
        if (!this.LastParameterIsCt(ctxt))
        {
            return base.InvokeTestMethod(ctxt, testClassInstance);
        }

        return Guard.ArgumentNotNull(ctxt).TestMethod.Invoke(testClassInstance, [.. ctxt.TestMethodArguments, TestContext.Current.CancellationToken]);
    }


    protected override ValueTask<TimeSpan> InvokeTest(
        XunitTestRunnerContext ctxt,
        object? testClassInstance)
    {
        if (!this.LastParameterIsCt(ctxt))
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
                        var result = InvokeTestMethod(ctxt, testClassInstance);
                        var valueTask = AsyncUtility.TryConvertToValueTask(result);
                        if (valueTask.HasValue)
                            await valueTask.Value;
                    }
                }
            )
        );
    }
}