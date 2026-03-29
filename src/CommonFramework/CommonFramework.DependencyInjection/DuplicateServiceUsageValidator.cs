using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.DependencyInjection;

/// <summary>
/// Validator that detects duplicate registrations of services which are used as single-instance dependencies in constructors,
/// including keyed services, to prevent runtime resolution issues in the DI container.
/// </summary>
public class DuplicateServiceUsageValidator(Type[] exceptServices) : IServiceCollectionValidator
{
    public DuplicateServiceUsageValidator()
        : this(Type.EmptyTypes)
    {
    }

    /// <inheritdoc />
    public ValidationResult Validate(IServiceCollection services, object? options)
    {
        var wrongMultiUsage = GetWrongMultiUsage(services);

        var filteredWrongMultiUsage = wrongMultiUsage.Where(pair => !exceptServices.Contains(pair.ServiceType)).ToList();

        var errors = filteredWrongMultiUsage.Select(pair =>
        {
            var keyedParts = pair.ActualServiceKey == null ? null : $" (ServiceKey: {pair.ActualServiceKey.Item1})";

            return $"The service {pair.ServiceType} ({nameof(pair.Lifetime)} = {pair.Lifetime}) {keyedParts} has been registered many times. There are services that use it in the constructor in a single instance: "
                   + string.Join(", ", pair.UsedFor.Select(usedService => usedService.ImplementationType));
        });

        return new ValidationResult(errors);
    }

    private static List<(Type ServiceType, ServiceLifetime Lifetime, Tuple<object?>? ActualServiceKey, List<ServiceDescriptor> UsedFor)> GetWrongMultiUsage(IServiceCollection serviceCollection)
    {
        var usedParametersRequest =

            from service in serviceCollection

            let actualImplementationType = service.IsKeyedService ? service.KeyedImplementationType : service.ImplementationType

            let actualImplementationFactory = service.IsKeyedService ? (object?)service.KeyedImplementationFactory : service.ImplementationFactory

            where actualImplementationType != null && actualImplementationFactory == null

            let ctors = actualImplementationType.GetConstructors()

            let actualCtor = ctors switch
            {
                [] => null,
                [var ctor] => ctor,
                _ => ctors.FirstOrDefault(ctor => ctor.GetCustomAttributes<ActivatorUtilitiesConstructorAttribute>().Any())
            }

            where actualCtor != null

            from parameter in actualCtor.GetParameters()

            let parameterType = parameter.ParameterType

            let parameterKey = parameter.GetCustomAttribute<FromKeyedServicesAttribute>()

            let actualParameterKey = parameterKey == null ? null : Tuple.Create(parameterKey.Key)

            group service by (parameterType, actualParameterKey, service.Lifetime);

        var usedParametersDict = usedParametersRequest.ToDictionary(g => g.Key, g => g.ToList());



        var wrongMultiUsageRequest =

            from service in serviceCollection

            let actualServiceKey = service.IsKeyedService ? Tuple.Create(service.ServiceKey) : null

            group service by (service.ServiceType, actualServiceKey, service.Lifetime) into serviceTypeGroup

            where serviceTypeGroup.Count() > 1

            let servicesWithSimpleUsage = usedParametersDict.GetValueOrDefault(serviceTypeGroup.Key)

            where servicesWithSimpleUsage != null

            select (serviceTypeGroup.Key.ServiceType, serviceTypeGroup.Key.Lifetime, serviceTypeGroup.Key.actualServiceKey, servicesWithSimpleUsage);

        return wrongMultiUsageRequest.ToList();
    }
}