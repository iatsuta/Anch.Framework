namespace Anch.Core.Auth;

public class FixedCurrentUser(string name) : ICurrentUser
{
    public string Name => name;

    public static FixedCurrentUser CurrentMachine { get; } = new(
        $"{Environment.MachineName}\\{Environment.UserName}");
}