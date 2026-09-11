using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Anch.GenericQueryable.EntityFramework;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseGenericQueryable(this DbContextOptionsBuilder optionsBuilder, Action<IEfGenericQueryableSetup>? setupAction = null)
    {
        var extension = optionsBuilder.Options.FindExtension<GenericQueryableOptionsExtension>()
                        ?? new GenericQueryableOptionsExtension(setupAction);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }
}