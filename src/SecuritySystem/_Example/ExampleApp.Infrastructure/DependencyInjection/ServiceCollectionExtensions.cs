using CommonFramework;

using ExampleApp.Application;
using ExampleApp.Domain;
using ExampleApp.Domain.Auth.Virtual;
using ExampleApp.Infrastructure.Services;
using AuthGeneral = ExampleApp.Domain.Auth.General;

using HierarchicalExpand;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SecuritySystem;
using SecuritySystem.DependencyInjection;
using SecuritySystem.GeneralPermission.DependencyInjection;
using SecuritySystem.Notification.DependencyInjection;
using SecuritySystem.UserSource;
using SecuritySystem.VirtualPermission.DependencyInjection;

namespace ExampleApp.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            return services
                .AddViews()
                .AddLogging()
                .AddHttpContextAccessor()
                .AddKeyedScoped<IInitializer, ExampleDataInitializer>(ExampleDataInitializer.Key)
                .AddKeyedSingleton<IInitializer, RootAppInitializer>(RootAppInitializer.Key)
                .AddSecuritySystem()
                .AddRepository();
        }

        private IServiceCollection AddRepository()
        {
            return services
                .AddScoped(typeof(IRepository<>), typeof(Repository<>))
                .AddScoped(typeof(IRepositoryFactory<>), typeof(RepositoryFactory<>));
        }

        private IServiceCollection AddSecuritySystem()
        {
            return services
                .AddSecuritySystem(sss =>
                    sss
                        .SetQueryableSource<DalQueryableSource>()
                        .SetGenericRepository<DalGenericRepository>()
                        .SetDefaultCurrentUser<ExampleDefaultCurrentUser>()
                        .SetDefaultCancellationTokenSource<ExampleDefaultCancellationTokenSource>()

                        .AddUserSource<Employee>()

                        .AddUserSource<AuthGeneral.Principal>(usb => usb
                            .SetRunAs(principal => principal.RunAs)
                            .SetMissedService<CreateVirtualMissedUserService<AuthGeneral.Principal>>())

                        .AddRunAsValidator<ExistsOtherwiseUsersRunAsValidator<AuthGeneral.Principal>>()

                        .AddSecurityContext<BusinessUnit>(
                            new Guid("{E4AE968E-7B6B-4236-B381-9886C8E0FA34}"),
                            scb => scb
                                .SetHierarchicalInfo(
                                    v => v.Parent,
                                    new AncestorLinkInfo<BusinessUnit, BusinessUnitDirectAncestorLink>(link => link.Ancestor, link => link.Child),
                                    new AncestorLinkInfo<BusinessUnit, BusinessUnitUndirectAncestorLink>(view => view.Source, view => view.Target),
                                    v => v.DeepLevel))

                        .AddSecurityContext<ManagementUnit>(
                            new Guid("{79296ADD-42D1-42C0-A4A9-254498A0C758}"),
                            scb => scb
                                .SetHierarchicalInfo(
                                    v => v.Parent,
                                    new AncestorLinkInfo<ManagementUnit, ManagementUnitDirectAncestorLink>(link => link.Ancestor, link => link.Child),
                                    new AncestorLinkInfo<ManagementUnit, ManagementUnitUndirectAncestorLink>(view => view.Source, view => view.Target),
                                    v => v.DeepLevel))

                        .AddSecurityContext<Employee>(
                            new Guid("{088D6453-C3AE-4421-9982-9C763DFD9054}"))

                        .AddSecurityContext<Location>(
                            new Guid("{9756440C-6643-4AAD-AB57-A901F3917BA4}"),
                            scb => scb.SetIdentityPath(loc => loc.MyId))

                        .AddDomainSecurity<TestObject>(b => b
                            .SetView(new[] { ExampleSecurityRole.TestManager, ExampleSecurityRole.BuManager })
                            .SetPath(SecurityPath<TestObject>.Create(testObj => testObj.BusinessUnit).And(testObj => testObj.ManagementUnit)
                                .And(testObj => testObj.Location)))

                        .AddDomainSecurity<Employee>(b => b
                            .SetView(DomainSecurityRule.CurrentUser))

                        .AddDomainSecurity<BusinessUnit>(b => b
                            .SetView(ExampleSecurityRole.TestManager.ToSecurityRule(HierarchicalExpandType.All))
                            .SetPath(SecurityPath<BusinessUnit>.Create(v => v)))

                        .AddSecurityRole(SecurityRole.Administrator,
                            new SecurityRoleInfo(new Guid("{2573CFDC-91CD-4729-AE97-82AB2F235E23}")))

                        .AddSecurityRole(ExampleSecurityRole.TestManager,
                            new SecurityRoleInfo(new Guid("{16EBA629-4319-4C15-AED3-032E4E09866D}")) { IsVirtual = true })

                        .AddSecurityRole(ExampleSecurityRole.BuManager,
                            new SecurityRoleInfo(new Guid("{72D24BB5-F661-446A-A458-53D301805971}"))
                                { Restriction = SecurityPathRestriction.Create<BusinessUnit>(true) })

                        .AddSecurityRole(ExampleSecurityRole.DefaultRole,
                            new SecurityRoleInfo(new Guid("{C6BE7D52-7F34-430C-9EEF-9CE6FD4D1FE5}")))

                        .AddSecurityRole(ExampleSecurityRole.NotificationRole,
                            new SecurityRoleInfo(new Guid("{E028CE61-C806-4603-B6B9-52E4DE302273}")))

                        .AddSecurityRole(ExampleSecurityRole.WithRestrictionFilterRole,
                            new SecurityRoleInfo(new Guid("{00645BD7-2D47-40E4-B542-E9A33EC06CB4}"))
                            {
                                Restriction = SecurityPathRestriction.Create<BusinessUnit>(required: true, filter: bu => bu.AllowedForFilterRole)
                            })

                        .AddVirtualPermission<Employee, TestManager>(
                            domainObject => domainObject.Employee,
                            b => b
                                .AddRestriction(domainObject => domainObject.BusinessUnit)
                                .AddRestriction(domainObject => domainObject.Location)
                                .AddSecurityRole(ExampleSecurityRole.TestManager))

                        .AddVirtualPermission<Employee, Administrator>(
                            domainObject => domainObject.Employee,
                            b => b.AddSecurityRole(SecurityRole.Administrator))

                        .AddGeneralPermission(
                            p => p.Principal,
                            p => p.SecurityRole,
                            (AuthGeneral.PermissionRestriction pr) => pr.Permission,
                            pr => pr.SecurityContextType,
                            pr => pr.SecurityContextId,
                            b => b
                                .SetSecurityRoleDescription(sr => sr.Description)
                                .SetPermissionPeriod(
                                    new PropertyAccessors<AuthGeneral.Permission, DateTime?>(
                                        v => v.StartDate,
                                        v => v.StartDate,
                                        (permission, startDate) => permission.StartDate = startDate ?? DateTime.MinValue),
                                    new PropertyAccessors<AuthGeneral.Permission, DateTime?>(v => v.EndDate))
                                .SetPermissionComment(v => v.Comment)
                                .SetPermissionDelegation(v => v.DelegatedFrom)
                                .SetCustomPermissionManagementService<CustomPermissionManagementService>())

                        .AddNotification());
        }

        private IServiceCollection AddViews()
        {
            return services
                .AddSingleton(sp => new CreateViewSql(
                    GetUndirectAncestorLinkTypeView(
                        typeof(BusinessUnitUndirectAncestorLink),
                        typeof(BusinessUnitDirectAncestorLink),
                        sp.GetService<ViewSchema>()?.Name)))
                .AddSingleton(sp => new CreateViewSql(
                    GetUndirectAncestorLinkTypeView(
                        typeof(ManagementUnitUndirectAncestorLink),
                        typeof(ManagementUnitDirectAncestorLink),
                        sp.GetService<ViewSchema>()?.Name)));
        }
    }

    private static string GetUndirectAncestorLinkTypeView(Type undirectAncestorLinkType, Type directAncestorLinkType, string? schema)
    {
        var schemaPrefix = schema == null ? "" : $"{schema}_";

        return @$"
CREATE VIEW {schemaPrefix}{undirectAncestorLinkType.Name}
AS
    SELECT ancestorId as sourceId, childId as targetId, Id AS Id
    FROM {schemaPrefix}{directAncestorLinkType.Name}
UNION
    SELECT
         childId as sourceId, ancestorId as targetId, Id as Id
    FROM {schemaPrefix}{directAncestorLinkType.Name}";
    }
}