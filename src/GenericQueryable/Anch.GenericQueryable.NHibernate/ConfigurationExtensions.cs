using NHibernate.Cfg;

namespace Anch.GenericQueryable.NHibernate;

public static class ConfigurationExtensions
{
    public static Configuration SetGenericQueryProvider(this Configuration cfg) => cfg.SetGenericQueryProvider<VisitedNHibQueryProvider>();

    public static Configuration SetGenericQueryProvider<TQueryProvider>(this Configuration cfg)
        where TQueryProvider : IVisitedNHibQueryProvider
    {
        cfg.SessionFactory().ParsingLinqThrough<TQueryProvider>();

        return cfg;
    }
}