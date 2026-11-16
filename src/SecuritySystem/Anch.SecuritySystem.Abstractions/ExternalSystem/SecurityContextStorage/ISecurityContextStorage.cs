namespace Anch.SecuritySystem.ExternalSystem.SecurityContextStorage;

/// <summary>Reads security context objects (business units, locations, ...) from the application's data.</summary>
public interface ISecurityContextStorage
{
    /// <summary>Returns the storage of one security context type.</summary>
    /// <param name="securityContextType">The security context type.</param>
    ITypedSecurityContextStorage GetTyped(Type securityContextType);
}