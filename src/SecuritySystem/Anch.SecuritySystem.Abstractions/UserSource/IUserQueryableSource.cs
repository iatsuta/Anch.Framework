namespace Anch.SecuritySystem.UserSource;

public interface IUserQueryableSource<out TUser>
{
	IQueryable<TUser> GetQueryable(UserCredential userCredential);

	IUserQueryableSource<User> ToSimple();
}