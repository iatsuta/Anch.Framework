using System.Linq.Expressions;
using Anch.Core;
using Anch.DependencyInjection;
using Anch.SecuritySystem.DependencyInjection;
using Anch.SecuritySystem.ExternalSystem;
using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anch.SecuritySystem.VirtualPermission.DependencyInjection;

public class VirtualPermissionRootSetup<TPrincipal, TPermission>(PropertyAccessors<TPermission, TPrincipal> principalAccessors)
    : IVirtualPermissionRootSetup<TPermission>, IServiceInitializer<ISecuritySystemSetup>
    where TPermission : class
    where TPrincipal : class
{
    private readonly List<VirtualPermissionSecurityRoleItemBindingInfo<TPermission>> itemBindingInfoList = [];

    private readonly List<Func<PermissionBindingInfo<TPermission, TPrincipal>, PermissionBindingInfo<TPermission, TPrincipal>>> permissionBindingInit = [];

    private readonly List<Func<VirtualPermissionBindingInfo<TPermission>, VirtualPermissionBindingInfo<TPermission>>> virtualBindingInit = [];

    public IVirtualPermissionRootSetup<TPermission> AddRestriction<TSecurityContext>(
        Expression<Func<TPermission, IEnumerable<TSecurityContext>>> path)
        where TSecurityContext : ISecurityContext
    {
        this.virtualBindingInit.Add(permissionBinding => permissionBinding with { Restrictions = [..permissionBinding.Restrictions, path] });

        return this;
    }

    public IVirtualPermissionRootSetup<TPermission> AddRestriction<TSecurityContext>(
        Expression<Func<TPermission, TSecurityContext?>> path)
        where TSecurityContext : ISecurityContext
    {
        this.virtualBindingInit.Add(permissionBinding => permissionBinding with { Restrictions = [..permissionBinding.Restrictions, path] });

        return this;
    }

    public IVirtualPermissionRootSetup<TPermission> SetPeriod(
        PropertyAccessors<TPermission, DateTime?>? startDatePropertyAccessor,
        PropertyAccessors<TPermission, DateTime?>? endDatePropertyAccessor)
    {
        this.permissionBindingInit.Add(permissionBinding => permissionBinding with { PermissionStartDate = startDatePropertyAccessor });
        this.permissionBindingInit.Add(permissionBinding => permissionBinding with { PermissionEndDate = endDatePropertyAccessor });

        return this;
    }

    public IVirtualPermissionRootSetup<TPermission> SetPeriod(
        Expression<Func<TPermission, DateTime?>>? startDatePath,
        Expression<Func<TPermission, DateTime?>>? endDatePath) =>
        this.SetPeriod(
            startDatePath == null ? null : new PropertyAccessors<TPermission, DateTime?>(startDatePath),
            endDatePath == null ? null : new PropertyAccessors<TPermission, DateTime?>(endDatePath));

    public IVirtualPermissionRootSetup<TPermission> SetComment(Expression<Func<TPermission, string>> commentPath)
    {
        this.permissionBindingInit.Add(permissionBinding => permissionBinding with { PermissionComment = commentPath.ToPropertyAccessors() });

        return this;
    }

    public IVirtualPermissionRootSetup<TPermission> SetPermissionDelegation(
        Expression<Func<TPermission, TPermission?>> newDelegatedFromPath)
    {
        this.permissionBindingInit.Add(permissionBinding => permissionBinding with { DelegatedFrom = newDelegatedFromPath.ToPropertyAccessors() });

        return this;
    }

    public IVirtualPermissionRootSetup<TPermission> AddSecurityRole(SecurityRole securityRole,
        Action<IVirtualBindingInfoSettingsSetup<TPermission>>? init = null)
    {
        var innerBuilder = new VirtualBindingInfoSettingsSetup<TPermission>();

        init?.Invoke(innerBuilder);

        var virtualBindingInfo = innerBuilder.Initialize(new VirtualPermissionSecurityRoleItemBindingInfo<TPermission> { SecurityRole = securityRole });

        this.itemBindingInfoList.Add(virtualBindingInfo);

        return this;
    }

    public void Initialize(ISecuritySystemSetup securitySystemSetup)
    {
        var bindingInfo = this.permissionBindingInit.Aggregate(new PermissionBindingInfo<TPermission, TPrincipal> { IsReadonly = true, Principal = principalAccessors },
                (state, f) => f(state));

        var virtualBindingInfo = this.virtualBindingInit.Aggregate(new VirtualPermissionBindingInfo<TPermission> { Items = [..this.itemBindingInfoList] },
            (state, f) => f(state));

        securitySystemSetup.AddExtensions(services =>
        {
            services.AddSingleton(bindingInfo);
            services.AddSingletonFrom<PermissionBindingInfo<TPermission>, PermissionBindingInfo<TPermission, TPrincipal>>();
            services.AddSingletonFrom<PermissionBindingInfo, PermissionBindingInfo<TPermission>>();

            services.AddSingletonFrom<VirtualPermissionBindingInfo<TPermission>, IVirtualPermissionBindingInfoValidator>(validator =>
            {
                validator.Validate(virtualBindingInfo);
                return virtualBindingInfo;
            });
            services.AddSingletonFrom<VirtualPermissionBindingInfo, VirtualPermissionBindingInfo<TPermission>>();

            services.AddSingleton<IPermissionTypedRestrictionBindingInfo<TPermission>, VirtualPermissionTypedRestrictionBindingInfo<TPermission>>();

            services.AddScopedServiceProxy<IPermissionSource<TPermission>, VirtualPermissionSource<TPrincipal, TPermission>>();

            services.AddScoped<IPrincipalSourceService, VirtualPrincipalSourceService<TPrincipal, TPermission>>();
            services.AddScoped<IPermissionSystemFactory, VirtualPermissionSystemFactory<TPermission>>();

            services.AddScoped<IPermissionSecurityRoleFilterFactory<TPermission>, VirtualPermissionSecurityRoleFilterFactory<TPermission>>();
            services.AddScoped<IPermissionFilterFactory<TPermission>, VirtualPermissionFilterFactory<TPermission>>();

            services.TryAddSingleton<IVirtualPermissionBindingInfoValidator, VirtualPermissionBindingInfoValidator>();
        });
    }
}