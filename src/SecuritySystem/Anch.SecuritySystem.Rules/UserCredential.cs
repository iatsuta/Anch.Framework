namespace Anch.SecuritySystem;

/// <summary>Identifies a user: by name, by identifier or by a full <see cref="User"/>. Strings, identifiers and users convert to it implicitly.</summary>
public abstract record UserCredential
{
    /// <summary>A user identified by name.</summary>
    /// <param name="Name">The user name.</param>
    public record NamedUserCredential(string Name) : UserCredential
    {
        public override string ToString() => this.Name;
    }

    /// <summary>A user identified by a security identifier.</summary>
    /// <param name="Identity">The user identifier.</param>
    public record IdentUserCredential(SecurityIdentity Identity) : UserCredential
    {
        public override string ToString() => this.Identity.GetId().ToString()!;
    }

    /// <summary>A user given as a complete <see cref="User"/>.</summary>
    /// <param name="User">The user.</param>
    public record FullUserCredential(User User) : UserCredential
    {
        public override string ToString() => this.User.Name;
    }


    public static implicit operator UserCredential(string? name)
    {
        return name is null ? null! : new NamedUserCredential(name);
    }

    public static implicit operator UserCredential(SecurityIdentity identity)
    {
        return new IdentUserCredential(identity);
    }

    public static implicit operator UserCredential(User user)
    {
        return new FullUserCredential(user);
    }

    public static implicit operator UserCredential(Guid id)
    {
        return (SecurityIdentity)id;
    }

    public static implicit operator UserCredential(int id)
    {
        return (SecurityIdentity)id;
    }
}