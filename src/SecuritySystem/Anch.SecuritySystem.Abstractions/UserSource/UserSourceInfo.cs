using System.Linq.Expressions;

using Anch.Core;

namespace Anch.SecuritySystem.UserSource;

public record UserSourceInfo<TUser>(PropertyAccessors<TUser, bool> Filter) : UserSourceInfo
{
    public UserSourceInfo(Expression<Func<TUser, bool>> filterPath)
        : this(new PropertyAccessors<TUser, bool>(filterPath))
    {
    }

    public override Type UserType { get; } = typeof(TUser);
}

public abstract record UserSourceInfo
{
    public abstract Type UserType { get; }
}