using System.Linq.Expressions;

namespace Anch.SecuritySystem.VirtualPermission.DependencyInjection;

/// <summary>Settings of one role of a virtual permission.</summary>
/// <typeparam name="TPermission">The domain entity the permission is computed from.</typeparam>
public interface IVirtualBindingInfoSettingsSetup<TPermission>
    where TPermission : notnull
{
    /// <summary>Limits the role to the entities that satisfy a condition.</summary>
    /// <param name="filter">The condition.</param>
    IVirtualBindingInfoSettingsSetup<TPermission> AddFilter(Expression<Func<TPermission, bool>> filter) => this.AddFilter(_ => filter);

    /// <summary>Limits the role to the entities that satisfy a condition built from the service provider.</summary>
    /// <param name="getFilter">Builds the condition.</param>
    IVirtualBindingInfoSettingsSetup<TPermission> AddFilter(Func<IServiceProvider, Expression<Func<TPermission, bool>>> getFilter);
}