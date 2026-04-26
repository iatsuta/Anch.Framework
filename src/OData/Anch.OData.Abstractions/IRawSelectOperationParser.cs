using Anch.OData.Domain;

namespace Anch.OData;

public interface IRawSelectOperationParser
{
    SelectOperation Parse(string input);
}