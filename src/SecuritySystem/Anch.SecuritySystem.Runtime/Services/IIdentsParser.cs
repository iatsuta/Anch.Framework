namespace Anch.SecuritySystem.Services;

/// <summary>Parses text identifiers.</summary>
public interface IIdentsParser<out TIdent> : IIdentsParser
{
    new TIdent[] Parse(IEnumerable<string> idents);

    Array IIdentsParser.Parse(IEnumerable<string> idents) => this.Parse(idents);
}

/// <summary>Parses text identifiers.</summary>
public interface IIdentsParser
{
    Array Parse(IEnumerable<string> idents);
}