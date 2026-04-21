namespace HierarchicalExpand.IntegrationTests.Environment.UndirectView;

public class UndirectedAncestorViewScriptProvider(
    IUndirectedAncestorViewScriptGenerator undirectedAncestorViewScriptGenerator,
    IEnumerable<FullAncestorLinkInfo> fullAncestorLinkInfoList) : IViewCreationScriptProvider
{
    public IEnumerable<string> GetScripts() =>

        fullAncestorLinkInfoList.Select(info =>
            undirectedAncestorViewScriptGenerator.GetScript(info.DirectedLinkType, info.UndirectedLinkType));
}