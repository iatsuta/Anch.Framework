namespace Anch.SecuritySystem;

public abstract record SecurityContextRestriction(bool Required, string? Key)
{
    public abstract Type SecurityContextType { get; }

    public abstract SecurityContextRestrictionFilterInfo? RawFilter { get; }
}

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
