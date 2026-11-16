namespace Anch.SecuritySystem.AccessDenied;

/// <summary>The default exception thrown when access is denied.</summary>
/// <param name="message">The error message.</param>
public class AccessDeniedException(string message) : SecuritySystemException(message);
