using System.Linq.Expressions;

using CommonFramework;

namespace SecuritySystem.VirtualPermission.DependencyInjection;

public class VirtualBindingInfoSettingsSetup<TPermission> : IVirtualBindingInfoSettingsSetup<TPermission>
    where TPermission : notnull
{
    private readonly List<Func<VirtualPermissionSecurityRoleItemBindingInfo<TPermission>, VirtualPermissionSecurityRoleItemBindingInfo<TPermission>>> initList = new();

    public IVirtualBindingInfoSettingsSetup<TPermission> AddFilter(
        Func<IServiceProvider, Expression<Func<TPermission, bool>>> getFilter)
    {
        this.initList.Add(v => v with { Filter = sp => v.Filter(sp).BuildAnd(getFilter(sp)) });

        return this;
    }

    public VirtualPermissionSecurityRoleItemBindingInfo<TPermission> Initialize(VirtualPermissionSecurityRoleItemBindingInfo<TPermission> virtualBindingInfo)
    {
        return this.initList.Aggregate(virtualBindingInfo, (v, f) => f(v));
    }
}