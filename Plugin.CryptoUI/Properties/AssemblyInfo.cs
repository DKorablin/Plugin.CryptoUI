using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("0d31a0ae-154e-4208-8e48-be90e8a22f69")]
[assembly: System.CLSCompliant(true)]

[assembly: AssemblyDescription("Certificate Generator/Splitter")]

/*if $(ConfigurationName) == Release (
..\..\..\..\ILMerge.exe  "/out:$(ProjectDir)..\bin\$(TargetFileName)" "$(TargetDir)$(TargetFileName)" "$(TargetDir)BouncyCastle.Crypto.dll" "/lib:..\..\..\SAL\bin"
)*/