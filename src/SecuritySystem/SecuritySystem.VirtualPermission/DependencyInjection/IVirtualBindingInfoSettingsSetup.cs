using System.Linq.Expressions;

namespace SecuritySystem.VirtualPermission.DependencyInjection;

public interface IVirtualBindingInfoSettingsSetup<TPermission>
    where TPermission : notnull
{
    IVirtualBindingInfoSettingsSetup<TPermission> AddFilter(Expression<Func<TPermission, bool>> filter) => this.AddFilter(_ => filter);

    IVirtualBindingInfoSettingsSetup<TPermission> AddFilter(Func<IServiceProvider, Expression<Func<TPermission, bool>>> getFilter);
}