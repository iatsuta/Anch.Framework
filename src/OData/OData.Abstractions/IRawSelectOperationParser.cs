using OData.Domain;

namespace OData;

public interface IRawSelectOperationParser
{
    SelectOperation Parse(string input);
}