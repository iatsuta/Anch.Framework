namespace Anch.HierarchicalExpand.IntegrationTests.Environment.UndirectView;

public interface IUndirectedAncestorViewScriptGenerator
{
    string GetScript(Type directAncestorLinkType, Type undirectAncestorLinkType);
}