using System.Reflection;
using SoundBending;
using MelonLoader;

// ^C+^V from iLikeMyAccountName along with SoundBending.BuildInfo
[assembly: AssemblyTitle(BuildInfo.Description)]
[assembly: AssemblyDescription(BuildInfo.Description)]
[assembly: AssemblyCompany(BuildInfo.Company)]
[assembly: AssemblyProduct(BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BuildInfo.Author)]
[assembly: AssemblyTrademark(BuildInfo.Company)]
[assembly: AssemblyVersion(BuildInfo.Version)]
[assembly: AssemblyFileVersion(BuildInfo.Version)]
[assembly: MelonInfo(typeof(SoundBending.Mod), BuildInfo.Name, BuildInfo.Version, BuildInfo.Author)]

[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]