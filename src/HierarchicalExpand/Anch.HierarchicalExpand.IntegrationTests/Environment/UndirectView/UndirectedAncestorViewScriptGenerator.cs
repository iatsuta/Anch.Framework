namespace Anch.HierarchicalExpand.IntegrationTests.Environment.UndirectView;

public class UndirectedAncestorViewScriptGenerator(ViewSchema? schema = null) : IUndirectedAncestorViewScriptGenerator
{
    public string GetScript(Type directAncestorLinkType, Type undirectAncestorLinkType)
    {
        var schemaPrefix = schema == null ? "" : $"{schema.Name}_";

        return @$"
CREATE VIEW {schemaPrefix}{undirectAncestorLinkType.Name}
AS
    SELECT ancestorId as sourceId, childId as targetId, Id AS Id
    FROM {schemaPrefix}{directAncestorLinkType.Name}
UNION
    SELECT
         childId as sourceId, ancestorId as targetId, lower(
        substr(hex(Id), 17, 16) || '-' ||
        substr(hex(Id), 13, 4) || '-' ||
        substr(hex(Id), 9, 4) || '-' ||
        substr(hex(Id), 5, 4) || '-' ||
        substr(hex(Id), 1, 4) || substr(hex(Id), 21, 12)
    ) as Id
    FROM {schemaPrefix}{directAncestorLinkType.Name}";
    }
}