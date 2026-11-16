using System.Collections.Immutable;

using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.Testing;

/// <summary>A permission described for a test: a role, a period and the security context objects it is limited to. Converts to <see cref="ExternalSystem.Management.ManagedPermission"/>.</summary>
public class TestPermission
{
    /// <summary>Creates a permission with a role.</summary>
    /// <param name="securityRole">The role.</param>
    public TestPermission(SecurityRole securityRole)
        : this()
    {
        this.SecurityRole = securityRole;
    }

    /// <summary>Creates an empty permission; set the role before use.</summary>
    public TestPermission()
    {
    }

    /// <summary>The granted role.</summary>
    public SecurityRole? SecurityRole { get; set; }

    /// <summary>The validity period.</summary>
    public PermissionPeriod Period { get; set; } = PermissionPeriod.Eternity;

    /// <summary>The allowed security context objects: identifiers grouped by security context type.</summary>
    public Dictionary<Type, Array> Restrictions { get; set; } = [];

    /// <summary>Application-specific values attached to the permission.</summary>
    public Dictionary<string, object> ExtendedData { get; set; } = [];

    /// <summary>A free-text comment.</summary>
    public string Comment { get; set; } = "";

    /// <summary>The permission identifier.</summary>
    public SecurityIdentity Identity { get; init; } = SecurityIdentity.Default;

    /// <summary>The identifier of the permission this one was delegated from.</summary>
    public SecurityIdentity DelegatedFrom { get; init; } = SecurityIdentity.Default;

    /// <summary>Returns the single allowed object of a context type, or <c>null</c> when none is set.</summary>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    /// <typeparam name="TIdent">The type of the identifier.</typeparam>
    public TypedSecurityIdentity<TIdent>? GetSingle<TSecurityContext, TIdent>()
        where TSecurityContext : ISecurityContext
        where TIdent : notnull
    {
        var arr = (TIdent[]?)this.Restrictions.GetValueOrDefault(typeof(TSecurityContext));

        var value = arr is null ? default : arr.Select(TIdent? (v) => v).SingleOrDefault();

        return value is null ? null : new TypedSecurityIdentity<TIdent>(value);
    }

    /// <summary>Limits the permission to one object of a context type; <c>null</c> removes the limit.</summary>
    /// <param name="value">The identifier of the object.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    /// <typeparam name="TIdent">The type of the identifier.</typeparam>
    public void SetSingle<TSecurityContext, TIdent>(TypedSecurityIdentity<TIdent>? value)
        where TSecurityContext : ISecurityContext
        where TIdent : notnull
    {
        if (value is null)
        {
            this.Restrictions.Remove(typeof(TSecurityContext));
        }
        else
        {
            this.Restrictions[typeof(TSecurityContext)] = new[] { value.Id };
        }
    }

    /// <summary>Returns the allowed objects of a context type.</summary>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    /// <typeparam name="TIdent">The type of the identifier.</typeparam>
    public TypedSecurityIdentity<TIdent>[] GetMany<TSecurityContext, TIdent>()
        where TSecurityContext : ISecurityContext
        where TIdent : notnull
    {
        var arr = (TIdent[]?)this.Restrictions.GetValueOrDefault(typeof(TSecurityContext));

        var value = arr ?? [];

        return value.Select(TypedSecurityIdentity.Create).ToArray();
    }

    /// <summary>Limits the permission to the given objects of a context type; an empty array removes the limit.</summary>
    /// <param name="value">The identifiers of the objects.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    /// <typeparam name="TIdent">The type of the identifier.</typeparam>
    public void SetMany<TSecurityContext, TIdent>(TypedSecurityIdentity<TIdent>[] value)
        where TSecurityContext : ISecurityContext
        where TIdent : notnull
    {
        if (value.Length == 0)
        {
            this.Restrictions.Remove(typeof(TSecurityContext));
        }
        else
        {
            this.Restrictions[typeof(TSecurityContext)] = value.Select(v => v.Id).ToArray();
        }
    }

    /// <summary>Converts the test permission to a managed permission; the identifier is applied as given.</summary>
    /// <exception cref="InvalidOperationException">The role is not set.</exception>
    public ManagedPermission ToManagedPermission() => new()
    {
        Identity = this.Identity,
        ForceApplyIdentity = true,
        SecurityRole = this.SecurityRole ?? throw new InvalidOperationException($"{nameof(this.SecurityRole)} not initialized"),
        Period = this.Period,
        IsVirtual = false,
        Comment = this.Comment,
        DelegatedFrom = this.DelegatedFrom,
        Restrictions = this.Restrictions.Where(pair => pair.Value.Length > 0).ToImmutableDictionary(),
        ExtendedData = this.ExtendedData.ToImmutableDictionary()
    };

    public static implicit operator ManagedPermission(TestPermission testPermissionBuilder) =>
        testPermissionBuilder.ToManagedPermission();
}