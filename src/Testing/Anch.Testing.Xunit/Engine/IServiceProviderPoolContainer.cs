namespace Anch.Testing.Xunit.Engine;

/// <summary>An object that knows the container pool of its test.</summary>
public interface IServiceProviderPoolContainer
{
    IServiceProviderPool? ServiceProviderPool { get; set; }
}