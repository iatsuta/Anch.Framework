using OData.Domain;

namespace OData;

public interface ISelectOperationParser
{
    SelectOperation<TDomainObject> Parse<TDomainObject>(string input);
}