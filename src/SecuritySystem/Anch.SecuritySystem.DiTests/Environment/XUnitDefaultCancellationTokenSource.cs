using Anch.Core;

namespace Anch.SecuritySystem.DiTests.Environment;

public class XUnitDefaultCancellationTokenSource : IDefaultCancellationTokenSource
{
    public CancellationToken CancellationToken => TestContext.Current.CancellationToken;
}