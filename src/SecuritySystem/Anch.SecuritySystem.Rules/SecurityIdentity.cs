namespace Anch.SecuritySystem;

/// <summary>Identifier of a security object (role, user, context object): a typed value such as a <see cref="Guid"/> or an <see cref="int"/>, or an untyped string.</summary>
public abstract record SecurityIdentity
{
    /// <summary><c>true</c> if the identifier is empty or the default value of its type.</summary>
    public abstract bool IsDefault { get; }

    /// <summary>Returns the underlying identifier value.</summary>
    public abstract object GetId();

    public static implicit operator SecurityIdentity(Guid id)
    {
        return (TypedSecurityIdentity)id;
    }

    public static implicit operator SecurityIdentity(int id)
    {
        return (TypedSecurityIdentity)id;
    }

    public override string? ToString() => this.GetId().ToString();

    /// <summary>The empty identifier.</summary>
    public static SecurityIdentity Default { get; } = new UntypedSecurityIdentity("");
}

/// <summary>An identifier kept as a string.</summary>
/// <param name="Id">The identifier text.</param>
public record UntypedSecurityIdentity(string Id) : SecurityIdentity
{
    public override bool IsDefault => string.IsNullOrEmpty(this.Id);

    public override object GetId() => this.Id;
}

/// <summary>An identifier that keeps the type of its value.</summary>
public abstract record TypedSecurityIdentity : SecurityIdentity
{
    /// <summary>The type of the identifier value.</summary>
    public abstract Type IdentType { get; }


    public static implicit operator TypedSecurityIdentity(Guid id)
    {
        return Create(id);
    }

    public static implicit operator TypedSecurityIdentity(int id)
    {
        return Create(id);
    }

    /// <summary>Creates a typed identifier.</summary>
    /// <param name="ident">The identifier value.</param>
    /// <typeparam name="TIdent">The type of the identifier value.</typeparam>
    public static TypedSecurityIdentity<TIdent> Create<TIdent>(TIdent ident)
        where TIdent : notnull
    {
        return new TypedSecurityIdentity<TIdent>(ident);
    }
}

/// <summary>An identifier with a value of type <typeparamref name="TIdent"/>.</summary>
/// <param name="Id">The identifier value.</param>
/// <typeparam name="TIdent">The type of the identifier value.</typeparam>
public record TypedSecurityIdentity<TIdent>(TIdent Id) : TypedSecurityIdentity
    where TIdent : notnull
{
    public override bool IsDefault => EqualityComparer<TIdent>.Default.Equals(this.Id, default);

    public override Type IdentType { get; } = typeof(TIdent);

    public override object GetId()
    {
        return this.Id;
    }
}