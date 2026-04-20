using Xunit.Internal;
using Xunit.v3;

namespace CommonFramework.Testing.Engine;

public class CommonTestCaseRunner : XunitTestCaseRunner
{
    public new static CommonTestCaseRunner Instance { get; } = new ();

    protected override ValueTask<RunSummary> RunTest(XunitTestCaseRunnerContext ctxt, IXunitTest test)
    {
        Guard.ArgumentNotNull(ctxt);
        Guard.ArgumentNotNull(test);

        return CommonTestRunner.Instance.Run(
            test,
            ctxt.MessageBus,
            ctxt.ConstructorArguments,
            ctxt.ExplicitOption,
            ctxt.Aggregator.Clone(),
            ctxt.CancellationTokenSource,
            ctxt.BeforeAfterTestAttributes
        );
    }
}