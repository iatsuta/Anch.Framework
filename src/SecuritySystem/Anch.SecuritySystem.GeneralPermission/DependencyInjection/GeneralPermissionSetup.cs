using System.Linq.Expressions;
using Anch.Core;
using Anch.DependencyInjection;
using Anch.SecuritySystem.DependencyInjection;
using Anch.SecuritySystem.ExternalSystem.Management;
using Anch.SecuritySystem.GeneralPermission.Initialize;
using Anch.SecuritySystem.GeneralPermission.Validation;
using Anch.SecuritySystem.GeneralPermission.Validation.Permission;
using Anch.SecuritySystem.GeneralPermission.Validation.PermissionRestriction;
using Anch.SecuritySystem.GeneralPermission.Validation.Principal;
using Anch.SecuritySystem.Services;
using Anch.SecuritySystem.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.GeneralPermission.DependencyInjection;

public class GeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>(
    PropertyAccessors<TPermission, TPrincipal> principalAccessors,
    PropertyAccessors<TPermission, TSecurityRole> securityRoleAccessors,
    PropertyAccessors<TPermissionRestriction, TPermission> permissionAccessors,
    PropertyAccessors<TPermissionRestriction, TSecurityContextType> securityContextTypeAccessors,
    PropertyAccessors<TPermissionRestriction, TSecurityContextObjectIdent> securityContextObjectIdAccessors)
    : IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction>, IServiceInitializer<ISecuritySystemSetup>
    where TPrincipal : class
    where TPermission : class, new()
    where TSecurityRole : class, new()
    where TPermissionRestriction : class, new()
    where TSecurityContextType : class, new()
    where TSecurityContextObjectIdent : notnull
{
    private PropertyAccessors<TPermission, DateTime?>? startDateAccessors;

    private PropertyAccessors<TPermission, DateTime?>? endDatedAccessors;

    private Expression<Func<TPermission, string>>? commentPath;

    private Expression<Func<TSecurityRole, string>>? descriptionPath;

    private Expression<Func<TPermission, TPermission?>>? delegatedFromPath;

    private bool? isReadonly;

    private Type permissionEqualityComparerType =
        typeof(PermissionEqualityComparer<TPermission, TSecurityRole, TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>);

    private Type permissionManagementServiceType =
        typeof(PermissionManagementService<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>);

    private TPermissionBindingInfo ApplyOptionalPaths<TPermissionBindingInfo>(TPermissionBindingInfo bindingInfo)
        where TPermissionBindingInfo : PermissionBindingInfo<TPermission, TPrincipal>
    {
        return bindingInfo
            .PipeMaybe(this.startDateAccessors, (b, v) => b with { PermissionStartDate = v })
            .PipeMaybe(this.endDatedAccessors, (b, v) => b with { PermissionEndDate = v })
            .PipeMaybe(this.commentPath, (b, v) => b with { PermissionComment = v.ToPropertyAccessors() })
            .PipeMaybe(this.delegatedFromPath, (b, v) => b with { DelegatedFrom = v.ToPropertyAccessors() })
            .PipeMaybe(this.isReadonly, (b, v) => b with { IsReadonly = v });
    }

    private TGeneralPermissionBindingInfo ApplyGeneralOptionalPaths<TGeneralPermissionBindingInfo>(TGeneralPermissionBindingInfo bindingInfo)
        where TGeneralPermissionBindingInfo : GeneralPermissionBindingInfo<TPermission, TSecurityRole>
    {
        return bindingInfo
            .PipeMaybe(this.descriptionPath, (b, v) => b with { SecurityRoleDescription = v.ToPropertyAccessors() });
    }

    public IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionPeriod(
        PropertyAccessors<TPermission, DateTime?>? startDatePropertyAccessor,
        PropertyAccessors<TPermission, DateTime?>? endDatePropertyAccessor)
    {
        this.startDateAccessors = startDatePropertyAccessor;
        this.endDatedAccessors = endDatePropertyAccessor;

        return this;
    }

    public IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionPeriod(
        Expression<Func<TPermission, DateTime?>>? startDatePath,
        Expression<Func<TPermission, DateTime?>>? endDatePath)
    {
        return this.SetPermissionPeriod(
            startDatePath == null ? null : new PropertyAccessors<TPermission, DateTime?>(startDatePath),
            endDatePath == null ? null : new PropertyAccessors<TPermission, DateTime?>(endDatePath));
    }

    public IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionComment(
        Expression<Func<TPermission, string>> newCommentPath)
    {
        this.commentPath = newCommentPath;

        return this;
    }

    public IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionDelegation(
        Expression<Func<TPermission, TPermission?>> newDelegatedFromPath)
    {
        this.delegatedFromPath = newDelegatedFromPath;

        return this;
    }

    public IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetSecurityRoleDescription(
        Expression<Func<TSecurityRole, string>>? newDescriptionPath)
    {
        this.descriptionPath = newDescriptionPath;

        return this;
    }

    public IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetReadonly(bool value = true)
    {
        this.isReadonly = value;

        return this;
    }

    public IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetCustomPermissionEqualityComparer<TComparer>()
        where TComparer : IPermissionEqualityComparer<TPermission, TPermissionRestriction>
    {
        this.permissionEqualityComparerType = typeof(TComparer);

        return this;
    }

    public IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetCustomPermissionManagementService<
        TPermissionManagementService>()
        where TPermissionManagementService : IPermissionManagementService<TPrincipal, TPermission, TPermissionRestriction>
    {
        this.permissionManagementServiceType = typeof(TPermissionManagementService);

        return this;
    }

    public void Initialize(ISecuritySystemSetup securitySystemSetup)
    {
        this.RegisterGeneralServices(securitySystemSetup);

        var bindingInfo = new PermissionBindingInfo<TPermission, TPrincipal>
        {
            Principal = principalAccessors,
        }.Pipe(this.ApplyOptionalPaths);

        var generalBindingInfo = new GeneralPermissionBindingInfo<TPermission, TSecurityRole>
        {
            SecurityRole = securityRoleAccessors,
        }.Pipe(this.ApplyGeneralOptionalPaths);

        var restrictionBindingInfo =
            new GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission>
            {
                Permission = permissionAccessors,
                SecurityContextType = securityContextTypeAccessors,
                SecurityContextObjectId = securityContextObjectIdAccessors
            };

        securitySystemSetup
            .AddPermissionSystem(sp => new GeneralPermissionSystemFactory(sp, bindingInfo))
            .AddExtensions(services =>
            {
                services
                    .AddSingletonFrom<
                        PermissionBindingInfo,
                        PermissionBindingInfo<TPermission>>()

                    .AddSingletonFrom<
                        PermissionBindingInfo<TPermission>,
                        PermissionBindingInfo<TPermission, TPrincipal>>()

                    .AddSingleton(bindingInfo)

                    .AddSingletonFrom<
                        GeneralPermissionBindingInfo,
                        GeneralPermissionBindingInfo<TPermission>>()

                    .AddSingletonFrom<
                        GeneralPermissionBindingInfo<TPermission>,
                        GeneralPermissionBindingInfo<TPermission, TSecurityRole>>()

                    .AddSingleton(generalBindingInfo)

                    .AddSingletonFrom<
                        GeneralPermissionRestrictionBindingInfo,
                        GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType>>()

                    .AddSingletonFrom<
                        GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType>,
                        GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>>()

                    .AddSingletonFrom<
                        GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>,
                        GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission>>()

                    .AddSingleton(restrictionBindingInfo)

                    .AddScoped<
                        IManagedPrincipalConverter<TPrincipal>,
                        ManagedPrincipalConverter<TPrincipal, TPermission, TPermissionRestriction>>()

                    .AddScoped<
                        IPrincipalSourceService,
                        GeneralPrincipalSourceService<TPrincipal, TPermission>>()

                    .AddScoped<
                        IPermissionTypedRestrictionBindingInfo<TPermission>, GeneralPermissionTypedRestrictionBindingInfo<TPermission,
                            TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>>()

                    .AddKeyedScoped<
                        IPermissionValidator<TPermission, TPermissionRestriction>,
                        PermissionDelegationValidator<TPrincipal, TPermission, TPermissionRestriction>>(ISecurityValidator.ElementKey)

                    .AddSingleton<
                        IPermissionRestrictionTypeFilterFactory<TPermissionRestriction>,
                        PermissionRestrictionTypeFilterFactory<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>>()

                    .AddScoped<
                        IPermissionRestrictionFilterFactory<TPermissionRestriction>,
                        PermissionRestrictionFilterFactory<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>>()

                    .AddScoped<
                        IRawPermissionConverter<TPermissionRestriction>,
                        RawPermissionConverter<TPermissionRestriction, TSecurityContextObjectIdent>>()

                    .AddSingleton<IPermissionSecurityRoleFilterFactory<TPermission>, GeneralPermissionSecurityRoleFilterFactory<TPermission>>()

                    .AddScoped<
                        IPermissionFilterFactory<TPermission>,
                        GeneralPermissionFilterFactory<TPermission, TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>>()

                    .AddScoped<
                        IDisplayPermissionService<TPermission, TPermissionRestriction>,
                        DisplayPermissionService<TPermission, TPermissionRestriction>>()

                    .AddKeyedSingleton<
                        IPermissionRestrictionValidator<TPermissionRestriction>,
                        AllowedTypePermissionRestrictionValidator<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission>>(ISecurityValidator.ElementKey)

                    .AddKeyedScoped<
                        IPermissionRestrictionValidator<TPermissionRestriction>,
                        ExistsPermissionRestrictionValidator<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>>(ISecurityValidator.ElementKey)

                    .AddKeyedScoped<
                        IPermissionRestrictionValidator<TPermissionRestriction>,
                        AllowedFilterPermissionRestrictionValidator<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent, TPermission>>(ISecurityValidator.ElementKey)

                    .AddScoped<
                        IPermissionRestrictionLoader<TPermission, TPermissionRestriction>,
                        PermissionRestrictionLoader<TPermission, TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>>()

                    .AddScoped<
                        IRawPermissionRestrictionLoader<TPermission>,
                        RawPermissionRestrictionLoader<TPermission, TPermissionRestriction>>()

                    .AddSingleton<
                        IPermissionRestrictionRawConverter<TPermissionRestriction>,
                        PermissionRestrictionRawConverter<TPermissionRestriction, TSecurityContextType, TSecurityContextObjectIdent>>()

                    .AddSingleton<
                        IPermissionSecurityRoleResolver<TPermission>,
                        PermissionSecurityRoleResolver<TPermission, TSecurityRole>>()

                    .AddSingleton<
                        IPermissionRestrictionSecurityContextTypeResolver<TPermissionRestriction>,
                        PermissionRestrictionSecurityContextTypeResolver<TPermissionRestriction, TSecurityContextType>>()

                    .AddScoped<
                        ISecurityRoleInitializer<TSecurityRole>,
                        SecurityRoleInitializer<TPermission, TSecurityRole>>()

                    .AddScoped<
                        ISecurityContextInitializer<TSecurityContextType>,
                        SecurityContextInitializer<TSecurityContextType>>();

                services.AddScoped(typeof(IPermissionEqualityComparer<TPermission, TPermissionRestriction>), this.permissionEqualityComparerType);

                services.AddScoped(typeof(IPermissionManagementService<TPrincipal, TPermission, TPermissionRestriction>), this.permissionManagementServiceType);
            });
    }

    private ISecuritySystemSetup RegisterGeneralServices(ISecuritySystemSetup settings)
    {
        return settings
            .AddExtensions(services =>
            {
                if (services.AlreadyInitialized<IGeneralPermissionBindingInfoSource, GeneralPermissionBindingInfoSource>())
                {
                    return;
                }

                services
                    .AddSingleton(typeof(IPermissionSecurityRoleByIdentsFilterFactory<>), typeof(PermissionSecurityRoleByIdentsFilterFactory<>))

                    .AddScoped<ISecurityRoleInitializer, SecurityRoleInitializer>()

                    .AddScoped<ISecurityContextInitializer, SecurityContextInitializer>()

                    .AddScoped(typeof(IPrincipalValidator<,,>), typeof(PrincipalRootValidator<,,>))
                    .AddKeyedScoped(typeof(IPrincipalValidator<,,>), ISecurityValidator.ElementKey, typeof(PrincipalUniquePermissionValidator<,,>))

                    .AddScoped(typeof(IPermissionValidator<,>), typeof(PermissionRootValidator<,>))
                    .AddKeyedSingleton(typeof(IPermissionValidator<,>), ISecurityValidator.ElementKey, typeof(PermissionRequiredContextValidator<,>))

                    .AddScoped(typeof(IPermissionRestrictionValidator<>), typeof(PermissionRestrictionRootValidator<>))

                    .AddSingleton<InitializerSettings>()

                    .AddSingleton<IGeneralPermissionBindingInfoSource, GeneralPermissionBindingInfoSource>()
                    .AddSingleton<IGeneralPermissionRestrictionBindingInfoSource, GeneralPermissionRestrictionBindingInfoSource>();
            })

            .SetPrincipalManagementService<GeneralPrincipalManagementService>();
    }
}