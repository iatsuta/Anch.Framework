using NHibernate.Cfg;

namespace Anch.GenericQueryable.NHibernate;

/// <summary>Plugs GenericQueryable into the NHibernate configuration.</summary>
public static class ConfigurationExtensions
{
    /// <summary>Makes NHibernate queries use <see cref="VisitedNHibQueryProvider"/>.</summary>
    /// <param name="cfg">The NHibernate configuration.</param>
    public static Configuration SetGenericQueryProvider(this Configuration cfg) => cfg.SetGenericQueryProvider<VisitedNHibQueryProvider>();

    /// <summary>Makes NHibernate queries use a custom query provider.</summary>
    /// <param name="cfg">The NHibernate configuration.</param>
    /// <typeparam name="TQueryProvider">The query provider type.</typeparam>
    public static Configuration SetGenericQueryProvider<TQueryProvider>(this Configuration cfg)
        where TQueryProvider : IVisitedNHibQueryProvider
    {
        cfg.SessionFactory().ParsingLinqThrough<TQueryProvider>();

        return cfg;
    }
}