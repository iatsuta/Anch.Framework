namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Lists the operations that the current user may perform.</summary>
public interface IAvailableSecurityOperationSource
{
    /// <summary>Returns the available operations.</summary>
    IAsyncEnumerable<SecurityOperation> GetAvailableSecurityOperations();
}