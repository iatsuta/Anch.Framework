namespace Anch.Testing;

public record ServiceProviderIndex(int Index)
{
    public bool IsMain => this.Index == -1;
}