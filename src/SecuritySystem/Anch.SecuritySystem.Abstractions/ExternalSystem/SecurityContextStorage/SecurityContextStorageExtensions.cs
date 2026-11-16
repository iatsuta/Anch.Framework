namespace Anch.SecuritySystem.ExternalSystem.SecurityContextStorage;

/// <summary>Extensions for <see cref="ISecurityContextStorage"/>.</summary>
public static class SecurityContextStorageExtensions
{
    /// <summary>Returns the storage of a context type cast to the typed interface.</summary>
    /// <param name="storage">The storage.</param>
    /// <param name="securityContextType">The security context type.</param>
    /// <typeparam name="TIdent">The type of the object identifier.</typeparam>
    public static ITypedSecurityContextStorage<TIdent> GetTyped<TIdent>(this ISecurityContextStorage storage,
        Type securityContextType)
        where TIdent : notnull
    {
        return (ITypedSecurityContextStorage<TIdent>)storage.GetTyped(securityContextType);
    }
}