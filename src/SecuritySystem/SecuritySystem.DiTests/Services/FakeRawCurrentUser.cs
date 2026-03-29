using CommonFramework.Auth;

namespace SecuritySystem.DiTests.Services;

public class FakeRawCurrentUser : ICurrentUser
{
    public string Name => throw new NotImplementedException();
}