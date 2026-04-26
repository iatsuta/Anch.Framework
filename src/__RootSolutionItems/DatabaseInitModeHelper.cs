public static class DatabaseInitModeHelper
{
    public static CommonFramework.Testing.Database.DatabaseInitMode DatabaseInitMode =>

#if DEBUG
        CommonFramework.Testing.Database.DatabaseInitMode.ReuseSnapshot
#else
        CommonFramework.Testing.Database.DatabaseInitMode.RebuildSnapshot
#endif
    ;
}