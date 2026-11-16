namespace Anch.Core.Auth;

/// <summary>The user on whose behalf the code runs. Several implementations are registered as keyed services with the keys below.</summary>
public interface ICurrentUser
{
    /// <summary>Key of the user taken from the environment (for example the authenticated Windows user).</summary>
    public const string RawKey = "Raw";

    /// <summary>Key of the user that is impersonated for the duration of a call.</summary>
    public const string ImpersonatedKey = "Impersonated";

    /// <summary>Key of the default current user that application code should use.</summary>
    public const string DefaultKey = "Default";

    /// <summary>The user name.</summary>
    string Name { get; }
}