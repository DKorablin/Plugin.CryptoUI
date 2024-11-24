using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("0d31a0ae-154e-4208-8e48-be90e8a22f69")]
[assembly: System.CLSCompliant(true)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://github.com/DKorablin/Plugin.CryptoUI")]
#else

[assembly: AssemblyTitle("Plugin.CryptoUI")]
[assembly: AssemblyDescription("Certificate Generator/Splitter")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin.CryptoUI")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2018-2019")]
#endif

/*if $(ConfigurationName) == Release (
..\..\..\..\ILMerge.exe  "/out:$(ProjectDir)..\bin\$(TargetFileName)" "$(TargetDir)$(TargetFileName)" "$(TargetDir)BouncyCastle.Crypto.dll" "/lib:..\..\..\SAL\bin"
)*/