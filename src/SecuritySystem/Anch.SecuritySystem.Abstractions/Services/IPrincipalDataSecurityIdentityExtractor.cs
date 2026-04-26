using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.Services;

public interface IPrincipalDataSecurityIdentityManager
{
    TypedSecurityIdentity Extract(PrincipalData principalData);
}