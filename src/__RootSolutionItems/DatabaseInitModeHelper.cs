using Anch.Testing.Database;

public static class DatabaseInitModeHelper
{
    public static DatabaseInitMode DatabaseInitMode =>

#if DEBUG
        DatabaseInitMode.RebuildSnapshot
#else
        DatabaseInitMode.RebuildSnapshot
#endif
    ;
}