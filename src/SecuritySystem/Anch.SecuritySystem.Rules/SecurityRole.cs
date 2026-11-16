namespace Anch.SecuritySystem;

/// <summary>A named role. Two roles are equal when their names are equal. Roles are registered with <c>AddSecurityRole</c>.</summary>
/// <param name="name">Name of the role.</param>
public class SecurityRole(string name)
{
    /// <summary>Name of the role.</summary>
    public string Name { get; } = name;

    public override string ToString() => this.Name;

    public override bool Equals(object? obj) => this.Equals(obj as SecurityRole);

    protected bool Equals(SecurityRole? other) => other is not null && this.Name == other.Name;

    public override int GetHashCode() => this.Name.GetHashCode();


    public static bool operator ==(SecurityRole? sr1, SecurityRole? sr2) =>
        ReferenceEquals(sr1, sr2) || (sr1 is not null && sr1.Equals(sr2));

    public static bool operator !=(SecurityRole? sr1, SecurityRole? sr2) => !(sr1 == sr2);

    /// <summary>
    /// Administrator role.
    /// </summary>
    public static SecurityRole Administrator { get; } = new(nameof(Administrator));

    /// <summary>
    /// System integration role.
    /// </summary>
    public static SecurityRole SystemIntegration { get; } = new(nameof(SystemIntegration));
}
