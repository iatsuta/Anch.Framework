using System.Linq.Expressions;

using Anch.Core;

namespace Anch.SecuritySystem.UserSource;

/// <summary>The property of the user entity that stores the user being run as.</summary>
public record UserSourceRunAsInfo<TUser>(PropertyAccessors<TUser, TUser?> RunAs)
{
    public UserSourceRunAsInfo(Expression<Func<TUser, TUser?>> runAsPath)
        : this(new PropertyAccessors<TUser, TUser?>(runAsPath))
    {
    }
}