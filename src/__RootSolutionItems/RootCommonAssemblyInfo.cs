using System.Reflection;

[assembly: AssemblyCompany("IvAt")]

[assembly: AssemblyVersion("3.2.0.0")]
[assembly: AssemblyInformationalVersion("changes at build")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif