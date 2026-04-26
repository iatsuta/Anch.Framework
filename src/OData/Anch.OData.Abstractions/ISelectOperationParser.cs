using Anch.OData.Domain;

namespace Anch.OData;

public interface ISelectOperationParser
{
    SelectOperation<TDomainObject> Parse<TDomainObject>(string input);
}