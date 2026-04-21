namespace ExampleApp.Infrastructure.DependencyInjection.UndirectView;

public interface IUndirectedAncestorViewScriptGenerator
{
    string GetScript(Type directAncestorLinkType, Type undirectAncestorLinkType);
}