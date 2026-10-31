using Anch.Core;

namespace Anch.SecuritySystem.Services;

/// <summary>Parses text identifiers of a parsable type, using the configured culture.</summary>
public class IdentsParser<TIdent>(ICultureSource? cultureSource = null) : IIdentsParser<TIdent>
    where TIdent : IParsable<TIdent>
{
    public TIdent[] Parse(IEnumerable<string> idents) => idents.Select(v => TIdent.Parse(v, cultureSource?.Culture)).ToArray();
}