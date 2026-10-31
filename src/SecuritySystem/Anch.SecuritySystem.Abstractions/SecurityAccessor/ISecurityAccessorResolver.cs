namespace Anch.SecuritySystem.SecurityAccessor;

/// <summary>Turns a description of who has access into the names of the users (accessors) it denotes.</summary>
public interface ISecurityAccessorResolver
{
    /// <summary>Key of the resolver that returns names as they are.</summary>
    public const string RawKey = "Raw";

    /// <summary>Returns the names of the accessors.</summary>
    /// <param name="data">The description of who has access.</param>
    IEnumerable<string> Resolve(SecurityAccessorData data);
}
