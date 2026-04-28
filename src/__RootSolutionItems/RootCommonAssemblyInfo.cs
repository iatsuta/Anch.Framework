using System.Reflection;

[assembly: AssemblyCompany("Anch")]
[assembly: AssemblyProduct("Anch.Framework")]

[assembly: AssemblyVersion("3.4.2.0")]
[assembly: AssemblyInformationalVersion("changes at build")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif