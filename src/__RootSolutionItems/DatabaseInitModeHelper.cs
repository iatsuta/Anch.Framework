public static class DatabaseInitModeHelper
{
    public static CommonFramework.Testing.Database.DatabaseInitMode DatabaseInitMode =>

#if DEBUG
        CommonFramework.Testing.Database.DatabaseInitMode.RebuildSnapshot
#else
        CommonFramework.Testing.Database.DatabaseInitMode.RebuildSnapshot
#endif
    ;
}