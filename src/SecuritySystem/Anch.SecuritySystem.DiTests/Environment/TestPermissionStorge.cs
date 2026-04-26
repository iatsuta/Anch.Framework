using Anch.SecuritySystem.Testing;

namespace Anch.SecuritySystem.DiTests.Environment;

public class TestPermissionStorge
{
    public List<TestPermission> Permissions { get; set; } = [];

    public void Reset() => this.Permissions = [];
}