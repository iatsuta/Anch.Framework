using Xunit.Internal;
using Xunit.v3;

namespace CommonFramework.Testing.Engine;

public class CommonTestMethodRunner : XunitTestMethodRunner
{
    public new static CommonTestMethodRunner Instance { get; } = new();

    protected override ValueTask<RunSummary> RunTestCase(XunitTestMethodRunnerContext ctxt, IXunitTestCase testCase)
    {
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