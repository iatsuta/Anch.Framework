using OData.Domain;

namespace OData;

public interface ISelectOperationConverter
{
    SelectOperation<TDomainObject> Convert<TDomainObject>(SelectOperation rawSelectOperation);
}