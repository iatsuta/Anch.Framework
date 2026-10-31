namespace Anch.SecuritySystem.ExternalSystem.SecurityContextStorage;

/// <summary>A security context object as shown to users: identifier, name and parent.</summary>
/// <param name="Id">The identifier.</param>
/// <param name="Name">The display name.</param>
/// <param name="ParentId">The identifier of the parent object; <c>null</c> for a root or a non-hierarchical type.</param>
/// <typeparam name="TSecurityContextIdent">The type of the identifier.</typeparam>
public record SecurityContextData<TSecurityContextIdent>(TSecurityContextIdent Id, string Name, TSecurityContextIdent? ParentId)
    where TSecurityContextIdent : notnull
{
    /// <summary>Returns the same data with untyped identifiers.</summary>
    public SecurityContextData<object> UpCast() => new(this.Id, this.Name, this.ParentId);
}