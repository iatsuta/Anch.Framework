namespace Anch.SecuritySystem;

/// <summary>Restricts a role to a security context type: which context types a permission may be limited by, whether the limit is mandatory, and which context objects are allowed.</summary>
/// <param name="Required">When <c>true</c>, a permission with this role must specify objects of this context type.</param>
/// <param name="Key">Distinguishes several restrictions of the same context type; <c>null</c> when there is only one.</param>
public abstract record SecurityContextRestriction(bool Required, string? Key)
{
    /// <summary>The restricted security context type.</summary>
    public abstract Type SecurityContextType { get; }

    /// <summary>The filter that limits the allowed context objects, or <c>null</c> when all are allowed.</summary>
    public abstract SecurityContextRestrictionFilterInfo? RawFilter { get; }
}

/// <summary>A typed <see cref="SecurityContextRestriction"/>.</summary>
/// <param name="Required">When <c>true</c>, a permission with this role must specify objects of this context type.</param>
/// <param name="Key">Distinguishes several restrictions of the same context type; <c>null</c> when there is only one.</param>
/// <param name="Filter">The filter that limits the allowed context objects, or <c>null</c> when all are allowed.</param>
/// <typeparam name="TSecurityContext">The security context type.</typeparam>
public record SecurityContextRestriction<TSecurityContext>(
    bool Required,
    string? Key,
    SecurityContextRestrictionFilterInfo<TSecurityContext>? Filter)
    : SecurityContextRestriction(Required, Key)
    where TSecurityContext : ISecurityContext
{
    private int? hashCode;

    protected SecurityContextRestriction(SecurityContextRestriction<TSecurityContext> source)
        : base(source)
    {
        this.Required = source.Required;
        this.Key = source.Key;
        this.Filter = source.Filter;
        this.SecurityContextType = source.SecurityContextType;
    }

    public override Type SecurityContextType { get; } = typeof(TSecurityContext);

    public override SecurityContextRestrictionFilterInfo? RawFilter => this.Filter;

    public virtual bool Equals(SecurityContextRestriction<TSecurityContext>? other) =>
        object.ReferenceEquals(this, other)
        || (other is not null
            && this.GetHashCode() == other.GetHashCode()
            && this.Required == other.Required
            && this.Key == other.Key
            && this.Filter == other.Filter);

    public override int GetHashCode() => this.hashCode ??= HashCode.Combine(this.Required, this.Key, this.Filter);
}
