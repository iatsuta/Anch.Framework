using Anch.Core;

namespace ExampleApp.IntegrationTests.Environment;

public class XUnitDefaultCancellationTokenSource : IDefaultCancellationTokenSource
{
    public CancellationToken CancellationToken => TestContext.Current.CancellationToken;
}