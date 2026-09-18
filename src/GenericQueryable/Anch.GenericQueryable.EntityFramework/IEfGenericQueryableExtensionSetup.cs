namespace Anch.GenericQueryable.EntityFramework;

public interface IEfGenericQueryableExtensionSetup
{
    IEfGenericQueryableExtensionSetup SetSetupType<TSetup>()
        where TSetup : IEfGenericQueryableExtensionInnerSetup, new();
}