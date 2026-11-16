namespace Anch.Testing.Xunit.Engine;

/// <summary>A service provider that returns its container to the pool when the test ends.</summary>
public class HandledServiceProvider : IServiceProvider
{
    private HandledServiceProvider()
    {
    }

    public object GetService(Type serviceType)
    {
        throw new InvalidOperationException("This service provider is not intended to be used.");
    }


    public static HandledServiceProvider Instance { get; } = new();
}