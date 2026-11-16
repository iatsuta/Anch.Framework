namespace Anch.SecuritySystem;

/// <summary>A user of the application.</summary>
/// <param name="Name">The user name.</param>
/// <param name="Identity">The user identifier.</param>
public record User(string Name, SecurityIdentity Identity);