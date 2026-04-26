using ExampleApp.Domain;

using FluentNHibernate.Mapping;

namespace ExampleApp.Infrastructure.Mapping;

public class LocationMap : ClassMap<Location>
{
    public LocationMap()
    {
        this.Schema("app");

        this.DynamicUpdate();

        this.Id(x => x.MyId).GeneratedBy.Identity();

        this.Map(x => x.Name).Not.Nullable();
    }
}