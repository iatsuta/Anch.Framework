using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Anch.GenericQueryable.EntityFramework;

/// <summary>Plugs GenericQueryable into Entity Framework Core.</summary>
public static class DbContextOptionsBuilderExtensions
{
    /// <summary>Makes the queries of the context support the ORM-independent operators and <c>WithFetch</c>.</summary>
    /// <param name="optionsBuilder">The options builder of the context.</param>
    /// <param name="setupAction">Optional settings, for example a custom setup type.</param>
    public static DbContextOptionsBuilder UseGenericQueryable(this DbContextOptionsBuilder optionsBuilder, Action<IEfGenericQueryableExtensionSetup>? setupAction = null)
    {
        var extension = optionsBuilder.Options.FindExtension<GenericQueryableOptionsExtension>()
                        ?? new GenericQueryableOptionsExtension(setupAction);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }
}