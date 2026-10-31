namespace Anch.SecuritySystem;

/// <summary>The base class of the security system errors.</summary>
/// <param name="message">The error message.</param>
public class SecuritySystemException(string message) : Exception(message);