namespace Anch.SecuritySystem.Services;

/// <summary>Creates converters of principal entities into managed principal headers.</summary>
public interface IManagedPrincipalHeaderConverterFactory<TPrincipal>
{
    IManagedPrincipalHeaderConverter<TPrincipal> Create(PermissionBindingInfo bindingInfo);
}