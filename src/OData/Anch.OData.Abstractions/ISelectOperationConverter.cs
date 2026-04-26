using Anch.OData.Domain;

namespace Anch.OData;

public interface ISelectOperationConverter
{
    SelectOperation<TDomainObject> Convert<TDomainObject>(SelectOperation rawSelectOperation);
}