using Anch.Core;

namespace Anch.GenericQueryable.IntegrationTests.Environment;

public interface ITestDataInitializer : IInitializer
{
    Guid TestObjId { get; }
}