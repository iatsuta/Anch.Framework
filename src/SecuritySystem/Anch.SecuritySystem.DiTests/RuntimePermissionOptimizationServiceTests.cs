using Anch.SecuritySystem.PermissionOptimization;

namespace Anch.SecuritySystem.DiTests;

public class RuntimePermissionOptimizationServiceTests
{
    private readonly RuntimePermissionOptimizationService service = new();

    [Fact]
    public void Optimize_SingleTypeGuidArrays_MergesDistinct()
    {
        var g1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var g2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var permissions = new List<Dictionary<Type, Array>>
        {
            new() { { typeof(string), new[] { g1 } } },
            new() { { typeof(string), new[] { g2 } } },
            new() { { typeof(string), new[] { g1 } } }
        };

        var result = this.service.Optimize(permissions).ToList();

        Assert.Single(result);
        var arr = (Guid[])result[0][typeof(string)];
        Assert.Equivalent(new[] { g1, g2 }, arr);
    }

    [Fact]
    public void Optimize_DifferentTypes_NoMerge()
    {
        var permissions = new List<Dictionary<Type, Array>>
        {
            new() { { typeof(string), new[] { Guid.NewGuid() } } },
            new() { { typeof(int), new[] { 42 } } }
        };

        var result = this.service.Optimize(permissions).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.ContainsKey(typeof(string)));
        Assert.Contains(result, d => d.ContainsKey(typeof(int)));
    }

    [Fact]
    public void Optimize_MixedDictionaries_LeavesComplexUntouched()
    {
        var g1 = Guid.NewGuid();
        var g2 = Guid.NewGuid();

        var permissions = new List<Dictionary<Type, Array>>
        {
            new() { { typeof(string), new[] { g1 } } },
            new() { { typeof(string), new[] { g2 } } },
            new()
            {
                { typeof(string), new[] { g1 } },
                { typeof(int), new[] { 1, 2, 3 } }
            }
        };

        var result = this.service.Optimize(permissions).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Keys.Count == 1 && d.ContainsKey(typeof(string)));
        Assert.Contains(result, d => d.Keys.Count == 2);
    }

    [Fact]
    public void Optimize_EmptyInput_ReturnsEmpty()
    {
        var result = this.service.Optimize(new List<Dictionary<Type, Array>>()).ToList();
        Assert.Empty(result);
    }

    [Fact]
    public void Optimize_IntArrays_MergesDistinct()
    {
        var permissions = new List<Dictionary<Type, Array>>
        {
            new() { { typeof(int), new[] { 1, 2 } } },
            new() { { typeof(int), new[] { 2, 3 } } }
        };

        var result = this.service.Optimize(permissions).ToList();

        Assert.Single(result);
        var arr = (int[])result[0][typeof(int)];
        Assert.Equivalent(new[] { 1, 2, 3 }, arr);
    }
}

