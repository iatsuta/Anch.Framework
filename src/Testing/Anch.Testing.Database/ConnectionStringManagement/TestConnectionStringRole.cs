namespace Anch.Testing.Database.ConnectionStringManagement;

/// <summary>The role a test database plays: the empty schema snapshot, the filled snapshot, or the private database of a pooled container.</summary>
public record TestConnectionStringRole(string Name)
{
    public static TestConnectionStringRole EmptySnapshot { get; } = new(nameof(EmptySnapshot));

    public static TestConnectionStringRole FilledSnapshot { get; } = new(nameof(FilledSnapshot));
}