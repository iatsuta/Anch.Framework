namespace Anch.SecuritySystem.ExternalSystem.Management;

public interface IRootPrincipalSourceService : IPrincipalSourceServiceBase
{
    IPrincipalSourceService ForPrincipal(Type principalType);
}