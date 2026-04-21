namespace HierarchicalExpand.IntegrationTests.Environment.UndirectView;

public interface IViewCreationScriptProvider
{
    IEnumerable<string> GetScripts();
}