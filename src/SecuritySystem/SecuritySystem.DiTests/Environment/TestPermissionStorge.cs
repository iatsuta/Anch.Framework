using SecuritySystem.Testing;

namespace SecuritySystem.DiTests.Environment;

public class TestPermissionStorge
{
    public List<TestPermission> Permissions { get; set; } = [];

    public void Reset() => this.Permissions = [];
}