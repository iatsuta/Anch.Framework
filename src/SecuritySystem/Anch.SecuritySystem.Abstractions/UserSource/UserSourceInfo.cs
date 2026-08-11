using System.Linq.Expressions;

namespace Anch.SecuritySystem.UserSource;

public record UserSourceInfo<TUser>(Expression<Func<TUser, bool>> FilterPath) : UserSourceInfo
{
    public Func<TUser, bool> FilterGetter { get; } = FilterPath.Compile();

    public override Type UserType { get; } = typeof(TUser);
}

public abstract record UserSourceInfo
{
    public abstract Type UserType { get; }
}