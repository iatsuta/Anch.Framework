using Anch.Testing.Database;

public static class DatabaseInitModeHelper
{
    public static DatabaseInitMode DatabaseInitMode =>

#if DEBUG
        DatabaseInitMode.ReuseSnapshot
#else
        DatabaseInitMode.RebuildSnapshot
#endif
    ;
}