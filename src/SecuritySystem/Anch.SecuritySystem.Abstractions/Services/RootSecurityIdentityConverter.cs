using System.Collections.Immutable;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Services;

/// <summary>Converts an identifier by trying the converters of all known identifier types; the identifier must be convertible to exactly one distinct result.</summary>
/// <param name="serviceProvider">Supplies the converters.</param>
/// <param name="types">The identifier types to try.</param>
public class RootSecurityIdentityConverter(IServiceProvider serviceProvider, IEnumerable<Type> types) : ISecurityIdentityConverter
{
    private readonly ImmutableArray<ISecurityIdentityConverter> converters =
    [
        ..types.Distinct().Select(identType =>
            (ISecurityIdentityConverter)serviceProvider.GetRequiredService(typeof(ISecurityIdentityConverter<>).MakeGenericType(identType)))
    ];

    /// <inheritdoc/>
    public TypedSecurityIdentity? TryConvert(SecurityIdentity securityIdentity)
    {
        var convertRequest =

            from converter in this.converters

            let tryConvertedIdentity = converter.TryConvert(securityIdentity)

            where tryConvertedIdentity is not null

            select tryConvertedIdentity;

        return convertRequest.Distinct().SingleOrDefault();
    }

    /// <inheritdoc/>
    public TypedSecurityIdentity Convert(SecurityIdentity securityIdentity)
    {
        return this.TryConvert(securityIdentity) ?? throw new ArgumentOutOfRangeException(nameof(securityIdentity));
    }
}