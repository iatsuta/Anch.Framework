namespace Anch.GenericQueryable.EntityFramework;

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