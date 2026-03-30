using CommonFramework.VisualIdentitySource;

using SecuritySystem.Credential;
using SecuritySystem.Services;

namespace SecuritySystem.UserSource;

public class CreateVirtualMissedUserService<TUser>(
    IMissedUserErrorSource missedUserErrorSource,
    IVisualIdentityInfo<TUser> visualIdentityInfo,
    IDefaultUserConverter<TUser> defaultUserConverter) : IMissedUserService<TUser>
    where TUser : class, new()
{
    public TUser GetUser(UserCredential userCredential)
    {
        if (userCredential is UserCredential.NamedUserCredential namedUserCredential)
        {
            var user = new TUser();
            visualIdentityInfo.Name.Setter(user, namedUserCredential.Name);
            return user;
        }
        else
        {
            throw missedUserErrorSource.GetNotFoundException(typeof(TUser), userCredential);
        }
    }

    public IMissedUserService<User> ToSimple()
    {
        return new SimpleCreateVirtualMissedUserService(uc => defaultUserConverter.ConvertFunc(this.GetUser(uc)));
    }

    private class SimpleCreateVirtualMissedUserService(Func<UserCredential, User> getUser) : IMissedUserService<User>
    {
        public User GetUser(UserCredential userCredential) => getUser(userCredential);

        public IMissedUserService<User> ToSimple()
        {
            return this;
        }
    }
}