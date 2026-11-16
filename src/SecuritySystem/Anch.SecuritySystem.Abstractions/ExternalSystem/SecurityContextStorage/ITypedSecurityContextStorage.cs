namespace Anch.SecuritySystem.ExternalSystem.SecurityContextStorage;

/// <summary>Reads the objects of one security context type with untyped identifiers.</summary>
public interface ITypedSecurityContextStorage
{
    /// <summary>Returns all objects of the type.</summary>
    IEnumerable<SecurityContextData<object>> GetSecurityContexts();

    /// <summary>Returns the objects with the given identifiers.</summary>
    /// <param name="securityContextIdents">An array of identifiers of the storage's identifier type.</param>
    IEnumerable<SecurityContextData<object>> GetSecurityContextsByIdents(Array securityContextIdents);

    /// <summary>Determines whether an object with the identifier exists.</summary>
    /// <param name="securityIdentity">The identifier.</param>
    bool IsExists(SecurityIdentity securityIdentity);
}

/// <summary>Reads the objects of one security context type.</summary>
/// <typeparam name="TSecurityContextIdent">The type of the object identifier.</typeparam>
public interface ITypedSecurityContextStorage<TSecurityContextIdent> : ITypedSecurityContextStorage
    where TSecurityContextIdent : notnull
{
    /// <summary>Returns all objects of the type.</summary>
    new IEnumerable<SecurityContextData<TSecurityContextIdent>> GetSecurityContexts();

    /// <summary>Returns the objects with the given identifiers.</summary>
    /// <param name="securityContextIdents">The identifiers.</param>
    IEnumerable<SecurityContextData<TSecurityContextIdent>> GetSecurityContextsByIdents(IEnumerable<TSecurityContextIdent> securityContextIdents);

    /// <summary>Returns the object and all its ancestors in the hierarchy.</summary>
    /// <param name="startSecurityEntityId">The identifier of the start object.</param>
    IEnumerable<SecurityContextData<TSecurityContextIdent>> GetSecurityContextsWithMasterExpand(TSecurityContextIdent startSecurityEntityId);

    /// <summary>Determines whether an object with the identifier exists.</summary>
    /// <param name="securityContextId">The identifier.</param>
    bool IsExists(TSecurityContextIdent securityContextId);
}
