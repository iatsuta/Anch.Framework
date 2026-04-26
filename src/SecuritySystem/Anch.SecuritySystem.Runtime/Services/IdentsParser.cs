using Anch.Core;

namespace Anch.SecuritySystem.Services;

public class IdentsParser<TIdent>(ICultureSource? cultureSource = null) : IIdentsParser<TIdent>
	where TIdent : IParsable<TIdent>
{
	public TIdent[] Parse(IEnumerable<string> idents) => idents.Select(v => TIdent.Parse(v, cultureSource?.Culture)).ToArray();
}