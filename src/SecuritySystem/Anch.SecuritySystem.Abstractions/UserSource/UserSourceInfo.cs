using System.Linq.Expressions;

namespace Anch.SecuritySystem.UserSource;

/// <summary>Settings of the user source of a user entity type.</summary>
/// <param name="FilterPath">The condition that selects the active users.</param>
/// <typeparam name="TUser">The user entity type.</typeparam>
public record UserSourceInfo<TUser>(Expression<Func<TUser, bool>> FilterPath) : UserSourceInfo
{
    public override Type UserType { get; } = typeof(TUser);
}

/// <summary>Untyped settings of a user source.</summary>
public abstract record UserSourceInfo
{
    /// <summary>The user entity type.</summary>
    public abstract Type UserType { get; }
}