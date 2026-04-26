namespace Anch.SecuritySystem.GeneralPermission;

public interface ISecurityContextTypeTypeResolver<in TSecurityContextType>
{
    Type Resolve(TSecurityContextType securityContextType);
}