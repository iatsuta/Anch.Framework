using System.Linq.Expressions;

using Anch.SecuritySystem.DomainServices;

namespace Anch.SecuritySystem.DependencyInjection.Domain;

public interface IDomainSecurityServiceSetup<TDomainObject>
{
    IDomainSecurityServiceSetup<TDomainObject> SetView(DomainSecurityRule securityRule) => this.SetMode(SecurityRule.View, securityRule);

    IDomainSecurityServiceSetup<TDomainObject> SetEdit(DomainSecurityRule securityRule) => this.SetMode(SecurityRule.Edit, securityRule);

    IDomainSecurityServiceSetup<TDomainObject> SetMode(SecurityRule.ModeSecurityRule modeSecurityRule, DomainSecurityRule implementedSecurityRule);

    IDomainSecurityServiceSetup<TDomainObject> SetPath(SecurityPath<TDomainObject> securityPath);

    /// <summary>
    /// RelativeDomainPathInfo must be registered
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    IDomainSecurityServiceSetup<TDomainObject> SetDependency<TSource>()
        where TSource : class;

    /// <summary>
    /// RelativeDomainPathInfo will be automatically registered
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="relativeDomainPath"></param>
    /// <returns></returns>
    IDomainSecurityServiceSetup<TDomainObject> SetDependency<TSource>(Expression<Func<TDomainObject, TSource>> relativeDomainPath)
        where TSource : class;

    /// <summary>
    /// RelativeDomainPathInfo will be automatically registered
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="relativeDomainPath"></param>
    /// <returns></returns>
    IDomainSecurityServiceSetup<TDomainObject> SetDependency<TSource>(Expression<Func<TDomainObject, IEnumerable<TSource>>> relativeDomainPath)
        where TSource : class;

    /// <summary>
    /// For projection
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    IDomainSecurityServiceSetup<TDomainObject> SetUntypedDependency<TSource>();

    IDomainSecurityServiceSetup<TDomainObject> SetCustomService<TDomainSecurityService>()
        where TDomainSecurityService : IDomainSecurityService<TDomainObject>;
}