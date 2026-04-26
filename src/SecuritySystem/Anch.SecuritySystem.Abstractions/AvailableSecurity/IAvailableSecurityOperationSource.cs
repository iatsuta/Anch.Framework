namespace Anch.SecuritySystem.AvailableSecurity;

public interface IAvailableSecurityOperationSource
{
    IAsyncEnumerable<SecurityOperation> GetAvailableSecurityOperations();
}