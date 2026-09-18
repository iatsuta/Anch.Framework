using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.EntityFramework;

public class GenericQueryableOptionsExtension(Action<IEfGenericQueryableExtensionSetup>? setupAction) : IDbContextOptionsExtension
{
    public DbContextOptionsExtensionInfo Info => field ??= new ExtensionInfo(this);

    private EfGenericQueryableExtensionSetup ExtensionSetup
    {
        get
        {
            if (field == null)
            {
                field = new EfGenericQueryableExtensionSetup();
                setupAction?.Invoke(field);
            }

            return field;
        }
    }

    public void ApplyServices(IServiceCollection services)
    {
        var innerSetup = new EfGenericQueryableSetup();

        this.ExtensionSetup.CreateInstance?.Invoke().Initialize(innerSetup);

        innerSetup.Initialize(services);
    }

    public void Validate(IDbContextOptions options)
    {
    }

    private sealed class ExtensionInfo(GenericQueryableOptionsExtension extension) : DbContextOptionsExtensionInfo(extension)
    {
        private EfGenericQueryableExtensionSetup ExtensionSetup => extension.ExtensionSetup;

        public override bool IsDatabaseProvider => false;

        public override string LogFragment => "using GenericQueryable ";

        public override int GetServiceProviderHashCode() => (this.ExtensionSetup.SetupType ?? typeof(ExtensionInfo)).GetHashCode();

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo untypedOther)
        {
            return untypedOther is ExtensionInfo other && this.ExtensionSetup.SetupType == other.ExtensionSetup.SetupType;
        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            debugInfo["GenericQueryable"] = this.GetServiceProviderHashCode().ToString();
        }
    }
}