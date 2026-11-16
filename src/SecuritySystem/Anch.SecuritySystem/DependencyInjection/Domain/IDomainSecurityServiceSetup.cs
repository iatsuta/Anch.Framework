using System.Linq.Expressions;

using Anch.SecuritySystem.DomainServices;

namespace Anch.SecuritySystem.DependencyInjection.Domain;

/// <summary>Fluent configuration of the access rules of one domain type.</summary>
/// <typeparam name="TDomainObject">The protected domain type.</typeparam>
public interface IDomainSecurityServiceSetup<TDomainObject>
{
    /// <summary>Sets the rule that grants viewing.</summary>
    /// <param name="securityRule">The rule.</param>
    IDomainSecurityServiceSetup<TDomainObject> SetView(DomainSecurityRule securityRule) => this.SetMode(SecurityRule.View, securityRule);

    /// <summary>Sets the rule that grants editing.</summary>
    /// <param name="securityRule">The rule.</param>
    IDomainSecurityServiceSetup<TDomainObject> SetEdit(DomainSecurityRule securityRule) => this.SetMode(SecurityRule.Edit, securityRule);

    /// <summary>Sets the rule that implements an access mode for the type.</summary>
    /// <param name="modeSecurityRule">The mode.</param>
    /// <param name="implementedSecurityRule">The rule that is checked for the mode.</param>
    IDomainSecurityServiceSetup<TDomainObject> SetMode(SecurityRule.ModeSecurityRule modeSecurityRule, DomainSecurityRule implementedSecurityRule);

    /// <summary>Sets how to reach the security contexts of an object.</summary>
    /// <param name="securityPath">The security path.</param>
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

    /// <summary>Replaces the domain security service of the type with a custom one.</summary>
    /// <typeparam name="TDomainSecurityService">The service type.</typeparam>
    IDomainSecurityServiceSetup<TDomainObject> SetCustomService<TDomainSecurityService>()
        where TDomainSecurityService : IDomainSecurityService<TDomainObject>;
}