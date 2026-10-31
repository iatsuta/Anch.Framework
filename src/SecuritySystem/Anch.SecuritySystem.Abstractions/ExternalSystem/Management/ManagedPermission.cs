using System.Collections.Immutable;

namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>A permission as managed through <see cref="IPrincipalManagementService"/>: a role, a validity period and restrictions by security contexts. A role converts to it implicitly.</summary>
public record ManagedPermission
{
    /// <summary>The granted role.</summary>
    public required SecurityRole SecurityRole { get; init; }

    /// <summary>Identifier of an existing permission; the default value for a new one.</summary>
    public SecurityIdentity Identity { get; init; } = SecurityIdentity.Default;

    /// <summary>Whether to use <see cref="Identity"/> for a new permission instead of generating one.</summary>
    public bool ForceApplyIdentity { get; init; }

    /// <summary>Whether the permission is computed rather than stored.</summary>
    public bool IsVirtual { get; init; }

    /// <summary>The validity period.</summary>
    public PermissionPeriod Period { get; init; } = PermissionPeriod.Eternity;

    /// <summary>A free-text comment.</summary>
    public string Comment { get; init; } = "";

    /// <summary>Identifier of the permission this one was delegated from; the default value when not delegated.</summary>
    public SecurityIdentity DelegatedFrom { get; init; } = SecurityIdentity.Default;

    /// <summary>The allowed security context objects: identifiers grouped by security context type.</summary>
    public ImmutableDictionary<Type, Array> Restrictions { get; init; } = [];

    /// <summary>Application-specific values attached to the permission.</summary>
    public ImmutableDictionary<string, object> ExtendedData { get; init; } = [];


    public static implicit operator ManagedPermission(SecurityRole securityRole) => new() { SecurityRole = securityRole };

    /// <summary>Returns a copy with an extended data value set.</summary>
    /// <param name="key">The name of the value.</param>
    /// <param name="value">The value.</param>
    public ManagedPermission WithExtendedData(string key, object value)
    {
        var newExtendedData = this.ExtendedData.ToDictionary();

        newExtendedData[key] = value;

        return this with { ExtendedData = newExtendedData.ToImmutableDictionary() };
    }
}