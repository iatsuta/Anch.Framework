namespace OData.Domain;

public interface ISelectOperationResult
{
    int TotalCount { get; }

    Type ElementType { get; }
}

public interface ISelectOperationResult<out T> : ISelectOperationResult
{
    IReadOnlyList<T> Items { get; }
}
