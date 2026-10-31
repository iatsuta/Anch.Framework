namespace Anch.SecuritySystem.Validation;

/// <summary>A value did not pass security validation.</summary>
/// <param name="message">The error message.</param>
public class SecuritySystemValidationException(string message) : SecuritySystemException(message);