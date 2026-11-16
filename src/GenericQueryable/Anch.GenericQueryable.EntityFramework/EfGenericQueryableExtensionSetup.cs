namespace Anch.GenericQueryable.EntityFramework;

/// <summary>Collects the settings of <c>UseGenericQueryable</c>.</summary>
public class EfGenericQueryableExtensionSetup : IEfGenericQueryableExtensionSetup
{
    public Func<IEfGenericQueryableExtensionInnerSetup>? CreateInstance { get; private set; }

    public Type? SetupType { get; private set; }

    public IEfGenericQueryableExtensionSetup SetSetupType<TSetup>()
        where TSetup : IEfGenericQueryableExtensionInnerSetup, new()
    {
        this.SetupType = typeof(TSetup);

        this.CreateInstance = () => new TSetup();

        return this;
    }
}