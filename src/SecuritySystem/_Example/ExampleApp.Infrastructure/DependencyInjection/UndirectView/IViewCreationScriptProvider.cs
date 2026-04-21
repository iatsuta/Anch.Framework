namespace ExampleApp.Infrastructure.DependencyInjection.UndirectView;

public interface IViewCreationScriptProvider
{
    IEnumerable<string> GetScripts();
}