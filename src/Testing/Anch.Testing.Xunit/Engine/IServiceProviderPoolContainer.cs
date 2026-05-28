namespace Anch.Testing.Xunit.Engine;

public interface IServiceProviderPoolContainer
{
    IServiceProviderPool? ServiceProviderPool { get; set; }
}