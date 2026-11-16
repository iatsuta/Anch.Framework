namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Finds the CLR type that a security context type entity stands for.</summary>
public interface ISecurityContextTypeTypeResolver<in TSecurityContextType>
{
    Type Resolve(TSecurityContextType securityContextType);
}