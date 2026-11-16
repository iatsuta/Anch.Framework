namespace Anch.SecuritySystem.Testing;

/// <summary>Whether test code in a scope reads or changes data.</summary>
public enum TestingScopeMode
{
    /// <summary>The code only reads data.</summary>
    Read = 0,

    /// <summary>The code changes data.</summary>
    Write = 1
}