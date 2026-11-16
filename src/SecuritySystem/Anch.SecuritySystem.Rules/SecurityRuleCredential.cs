namespace Anch.SecuritySystem;

/// <summary>Says whose permissions a rule is checked against.</summary>
public abstract record SecurityRuleCredential
{
    /// <summary>The current user; when a RunAs session is active, the user being run as.</summary>
    public record CurrentUserWithRunAsCredential : SecurityRuleCredential;

    /// <summary>The current user, ignoring any RunAs session.</summary>
    public record CurrentUserWithoutRunAsCredential : SecurityRuleCredential;

    /// <summary>Any user: the check is not tied to a particular user.</summary>
    public record AnyUserCredential : SecurityRuleCredential;

    /// <summary>A specific user.</summary>
    /// <param name="UserCredential">The user whose permissions are checked.</param>
    public record CustomUserSecurityRuleCredential(UserCredential UserCredential) : SecurityRuleCredential;

    /// <summary>Converts a user credential into a credential for a specific user; <c>null</c> stays <c>null</c>.</summary>
    /// <param name="userCredential">The user credential.</param>
    public static implicit operator SecurityRuleCredential(UserCredential? userCredential)
    {
        return userCredential is null ? null! : new CustomUserSecurityRuleCredential(userCredential);
    }

    /// <summary>Converts a user name into a credential for that user; <c>null</c> stays <c>null</c>.</summary>
    /// <param name="userCredential">The user name.</param>
    public static implicit operator SecurityRuleCredential(string? userCredential)
    {
        return (UserCredential?)userCredential;
    }
}