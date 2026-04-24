using CommonFramework;

namespace GenericQueryable.IntegrationTests.Environment;

public interface ISharedTestDataInitializer : IInitializer
{
    Guid TestObjId { get; }
}