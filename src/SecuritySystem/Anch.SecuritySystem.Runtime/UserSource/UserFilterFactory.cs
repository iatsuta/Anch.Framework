using System.Linq.Expressions;
using Anch.Core;
using Anch.IdentitySource;
using Anch.SecuritySystem.Services;
using Anch.VisualIdentitySource;

namespace Anch.SecuritySystem.UserSource;

public class UserFilterFactory<TUser>(
    IServiceProxyFactory serviceProxyFactory,
    IIdentityInfo<TUser> identityInfo) : IUserFilterFactory<TUser>
{
    private readonly Lazy<IUserFilterFactory<TUser>> lazyInnerService = new(() => serviceProxyFactory.Create<IUserFilterFactory<TUser>>(
        typeof(UserFilterFactory<,>).MakeGenericType(typeof(TUser), identityInfo.IdentityType)));

    public Expression<Func<TUser, bool>> CreateFilter(UserCredential userCredential) => this.lazyInnerService.Value.CreateFilter(userCredential);
}

public class UserFilterFactory<TUser, TIdent>(
    IIdentityInfo<TUser, TIdent> identityInfo,
    IVisualIdentityInfo<TUser> visualIdentityInfo,
    ISecurityIdentityConverter<TIdent> identityConverter) : IUserFilterFactory<TUser>
    where TUser : class
    where TIdent : notnull
{
    public Expression<Func<TUser, bool>> CreateFilter(UserCredential userCredential)
    {
        switch (userCredential)
        {
            case UserCredential.NamedUserCredential { Name: var name }:
                return visualIdentityInfo.Name.Path.Select(objName => objName == name);

            case UserCredential.IdentUserCredential { Identity: var identity }:
            {
                var convertedIdentity = identityConverter.TryConvert(identity);

                if (convertedIdentity is null)
                {
                    return _ => false;
                }
                else
                {
                    return identityInfo.Id.Path.Select(ExpressionHelper.GetEqualityWithExpr(convertedIdentity.Id));
                }
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(userCredential));
        }
    }
}