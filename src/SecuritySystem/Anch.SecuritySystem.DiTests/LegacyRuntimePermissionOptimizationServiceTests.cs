using Anch.SecuritySystem.PermissionOptimization;

namespace Anch.SecuritySystem.DiTests;

public class LegacyRuntimePermissionOptimizationServiceTests
{
    private readonly LegacyRuntimePermissionOptimizationService service = new();

    [Fact]
    public void Optimize_SingleTypeMultipleArrays_MergesAndDistincts()
    {
        var permissions = new List<Dictionary<Type, Array>>
        {
            new() { { typeof(string), new[] { Guid.Parse("11111111-1111-1111-1111-111111111111") } } },
            new() { { typeof(string), new[] { Guid.Parse("22222222-2222-2222-2222-222222222222") } } },
            new() { { typeof(string), new[] { Guid.Parse("11111111-1111-1111-1111-111111111111") } } }
        };

        var result = this.service.Optimize(permissions).ToList();

        Assert.Single(result);
        var array = (Guid[])result[0][typeof(string)];
        Assert.Equivalent(new[]
        {
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("22222222-2222-2222-2222-222222222222")
        }, array);
    }

    [Fact]
    public void Optimize_DifferentTypes_DoNotMerge()
    {
        var permissions = new List<Dictionary<Type, Array>>
        {
            new() { { typeof(string), new[] { Guid.NewGuid() } } },
            new() { { typeof(int), new[] { Guid.NewGuid() } } }
        };

        var result = this.service.Optimize(permissions).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.ContainsKey(typeof(string)));
        Assert.Contains(result, d => d.ContainsKey(typeof(int)));
    }

    [Fact]
    public void Optimize_MixedDictionaries_LeavesComplexUntouched()
    {
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();

        var permissions = new List<Dictionary<Type, Array>>
        {
            new() { { typeof(string), new[] { guid1 } } },
            new() { { typeof(string), new[] { guid2 } } },
            new()
            {
                { typeof(string), new[] { guid1 } },
                { typeof(int), new[] { guid2 } }
            }
        };

        var result = this.service.Optimize(permissions).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Keys.Count == 1 && d.ContainsKey(typeof(string)));
        Assert.Contains(result, d => d.Keys.Count == 2);
    }

    [Fact]
    public void Optimize_NoPermissions_ReturnsEmpty()
    {
        var result = this.service.Optimize(new List<Dictionary<Type, Array>>()).ToList();
        Assert.Empty(result);
    }
}