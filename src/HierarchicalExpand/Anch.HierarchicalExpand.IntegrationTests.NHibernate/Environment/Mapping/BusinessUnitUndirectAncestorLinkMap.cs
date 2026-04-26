using Anch.HierarchicalExpand.IntegrationTests.Domain;

using FluentNHibernate.Mapping;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment.Mapping;

public class BusinessUnitUndirectAncestorLinkMap : ClassMap<BusinessUnitUndirectAncestorLink>
{
    public BusinessUnitUndirectAncestorLinkMap()
    {
        this.Schema("app");

        this.SchemaAction.None();

        this.Id(x => x.Id).GeneratedBy.Assigned();

        this.References(x => x.Source).Column(nameof(BusinessUnitUndirectAncestorLink.Source) + "Id").Not.Nullable();
        this.References(x => x.Target).Column(nameof(BusinessUnitUndirectAncestorLink.Target) + "Id").Not.Nullable();
    }
}