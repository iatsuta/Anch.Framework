using Anch.Core;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public interface ISharedTestDataInitializer : IInitializer
{
    Guid TestObjId { get; }
}