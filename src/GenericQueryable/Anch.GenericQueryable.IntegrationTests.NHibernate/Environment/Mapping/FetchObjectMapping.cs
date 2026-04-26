using Anch.GenericQueryable.IntegrationTests.Domain;
using FluentNHibernate.Mapping;

namespace Anch.GenericQueryable.IntegrationTests.Environment.Mapping;

public class FetchObjectMapping : ClassMap<FetchObject>
{
    public FetchObjectMapping()
    {
        this.Schema("app");

        this.DynamicUpdate();

        this.Id(x => x.Id).GeneratedBy.GuidComb();
    }
}