namespace Anch.SecuritySystem.GeneralPermission.Initialize;

/// <summary>Settings of the initializers of stored roles and security context types.</summary>
public record InitializerSettings(UnexpectedSecurityElementMode UnexpectedSecurityElementMode = UnexpectedSecurityElementMode.Remove);
