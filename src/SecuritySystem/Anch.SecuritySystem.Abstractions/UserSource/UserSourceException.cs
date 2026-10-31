namespace Anch.SecuritySystem.UserSource;

/// <summary>A user or principal could not be found or resolved.</summary>
/// <param name="message">The error message.</param>
public class UserSourceException(string message) : SecuritySystemException(message);
