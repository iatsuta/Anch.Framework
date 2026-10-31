namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>Reads the principals of one principal entity type.</summary>
public interface IPrincipalSourceService : IPrincipalSourceServiceBase
{
    /// <summary>The principal entity type.</summary>
    Type PrincipalType { get; }
}