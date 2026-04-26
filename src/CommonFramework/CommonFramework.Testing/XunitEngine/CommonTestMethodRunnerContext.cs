using Xunit.Sdk;
using Xunit.v3;

namespace CommonFramework.Testing.XunitEngine;

public class CommonTestMethodRunnerContext(
    IXunitTestMethod testMethod,
    IReadOnlyCollection<IXunitTestCase> testCases,
    ExplicitOption explicitOption,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    object?[] constructorArguments,
    IServiceProvider? serviceProvider)
    : XunitTestMethodRunnerContext(testMethod, testCases, explicitOption, messageBus, aggregator, cancellationTokenSource, constructorArguments)
{
    public IServiceProvider? ServiceProvider { get; } = serviceProvider;
}