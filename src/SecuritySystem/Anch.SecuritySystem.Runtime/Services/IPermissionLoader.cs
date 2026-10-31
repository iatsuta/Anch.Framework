namespace Anch.SecuritySystem.Services;

/// <summary>Loads the permissions of a principal.</summary>
public interface IPermissionLoader<in TPrincipal, out TPermission>
{
    IAsyncEnumerable<TPermission> LoadAsync(TPrincipal principal);
}