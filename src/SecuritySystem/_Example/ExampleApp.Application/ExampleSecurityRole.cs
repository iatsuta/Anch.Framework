using Anch.SecuritySystem;

namespace ExampleApp.Application;

public static class ExampleSecurityRole
{
    public static SecurityRole TestManager { get; } = new (nameof(TestManager));

    public static SecurityRole BuManager { get; } = new(nameof(BuManager));

    public static SecurityRole DefaultRole { get; } = new(nameof(DefaultRole));

    public static SecurityRole WithRestrictionFilterRole { get; } = new(nameof(WithRestrictionFilterRole));

    public static SecurityRole NotificationRole { get; } = new(nameof(NotificationRole));
}