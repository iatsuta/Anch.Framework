namespace Anch.GenericQueryable.EntityFramework;

/// <summary>Settings of <c>UseGenericQueryable</c>.</summary>
public interface IEfGenericQueryableExtensionSetup
{
    /// <summary>Sets the type that configures GenericQueryable for the context, for example to register fetch rules.</summary>
    /// <typeparam name="TSetup">The setup type; it needs a parameterless constructor.</typeparam>
    IEfGenericQueryableExtensionSetup SetSetupType<TSetup>()
        where TSetup : IEfGenericQueryableExtensionInnerSetup, new();
}