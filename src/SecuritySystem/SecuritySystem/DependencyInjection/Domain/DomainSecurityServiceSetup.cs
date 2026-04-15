using System.Linq.Expressions;

using CommonFramework.DependencyInjection;
using CommonFramework.RelativePath;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.DomainServices;
using SecuritySystem.DomainServices.DependencySecurity;
using SecuritySystem.SecurityRuleInfo;

namespace SecuritySystem.DependencyInjection.Domain;

public abstract class DomainSecurityServiceSetup : IServiceInitializer
{
    public abstract Type DomainType { get; }

    public abstract void Initialize(IServiceCollection services);
}

public class DomainSecurityServiceSetup<TDomainObject> : DomainSecurityServiceSetup, IDomainSecurityServiceSetup<TDomainObject>
{
    private readonly Dictionary<SecurityRule.ModeSecurityRule, DomainSecurityRule> domainObjectSecurityDict = [];

    private SecurityPath<TDomainObject>? securityPath;

    private (Type Type, object Instance)? relativePathData;

    private Type serviceType = typeof(DomainSecurityService<TDomainObject>);

    public override Type DomainType { get; } = typeof(TDomainObject);

    public override void Initialize(IServiceCollection services)
    {
        foreach (var (modeSecurityRule, implementedSecurityRule) in this.domainObjectSecurityDict)
        {
            services.AddSingleton(new DomainModeSecurityRuleInfo(modeSecurityRule.ToDomain(this.DomainType), implementedSecurityRule));
        }

        if (this.securityPath != null)
        {
            services.AddSingleton(this.securityPath);
        }

        if (this.relativePathData is { } pair)
        {
            services.AddSingleton(pair.Type, pair.Instance);
        }

        services.AddScoped(typeof(IDomainSecurityService<TDomainObject>), this.serviceType);
    }

    public IDomainSecurityServiceSetup<TDomainObject> SetMode(SecurityRule.ModeSecurityRule modeSecurityRule, DomainSecurityRule implementedSecurityRule)
    {
        this.domainObjectSecurityDict[modeSecurityRule] = implementedSecurityRule;

        return this;
    }

    public IDomainSecurityServiceSetup<TDomainObject> SetPath(SecurityPath<TDomainObject> newSecurityPath)
    {
        this.securityPath = newSecurityPath;

        return this;
    }

    public IDomainSecurityServiceSetup<TDomainObject> SetDependency<TSource>()
        where TSource : class
    {
        this.serviceType = typeof(DependencyDomainSecurityService<TDomainObject, TSource>);

        return this;
    }

    public IDomainSecurityServiceSetup<TDomainObject> SetDependency<TSource>(Expression<Func<TDomainObject, TSource>> relativeDomainPath)
        where TSource : class
    {
        return this.SetDependency(new SingleRelativeDomainPathInfo<TDomainObject, TSource>(relativeDomainPath));
    }

    public IDomainSecurityServiceSetup<TDomainObject> SetDependency<TSource>(Expression<Func<TDomainObject, IEnumerable<TSource>>> relativeDomainPath)
        where TSource : class
    {
        return this.SetDependency(new ManyRelativeDomainPathInfo<TDomainObject, TSource>(relativeDomainPath));
    }

    private IDomainSecurityServiceSetup<TDomainObject> SetDependency<TSource>(IRelativeDomainPathInfo<TDomainObject, TSource> relativeDomainPathInfo)
        where TSource : class
    {
        this.SetDependency<TSource>();

        this.relativePathData = (typeof(IRelativeDomainPathInfo<TDomainObject, TSource>), relativeDomainPathInfo);

        return this;
    }

    public IDomainSecurityServiceSetup<TDomainObject> SetUntypedDependency<TSource>()
    {
        this.serviceType = typeof(UntypedDependencyDomainSecurityService<,>).MakeGenericType(typeof(TDomainObject), typeof(TSource));

        return this;
    }

    public IDomainSecurityServiceSetup<TDomainObject> SetCustomService<TDomainSecurityService>()
        where TDomainSecurityService : IDomainSecurityService<TDomainObject>
    {
        this.serviceType = typeof(TDomainSecurityService);

        return this;
    }
}