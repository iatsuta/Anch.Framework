namespace Anch.SecuritySystem.VirtualPermission;

/// <summary>Validates the configuration of a virtual permission.</summary>
public interface IVirtualPermissionBindingInfoValidator
{
    void Validate(VirtualPermissionBindingInfo virtualBindingInfo);
}