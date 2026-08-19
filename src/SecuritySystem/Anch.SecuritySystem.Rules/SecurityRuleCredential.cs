namespace Anch.SecuritySystem;

public abstract record SecurityRuleCredential
{
    public record CurrentUserWithRunAsCredential : SecurityRuleCredential;

    public record CurrentUserWithoutRunAsCredential : SecurityRuleCredential;

    public record AnyUserCredential : SecurityRuleCredential;

    public record CustomUserSecurityRuleCredential(UserCredential UserCredential) : SecurityRuleCredential;

    public static implicit operator SecurityRuleCredential(UserCredential? userCredential)
    {
        return userCredential is null ? null! : new CustomUserSecurityRuleCredential(userCredential);
    }

    public static implicit operator SecurityRuleCredential(string? userCredential)
    {
        return (UserCredential?)userCredential;
    }
}