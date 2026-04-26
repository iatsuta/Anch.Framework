namespace Anch.Core.Auth;

public interface ICurrentUser
{
    public const string RawKey = "Raw";

    public const string ImpersonatedKey = "Impersonated";

    public const string DefaultKey = "Default";

    string Name { get; }
}