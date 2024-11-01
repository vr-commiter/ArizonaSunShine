using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(ArizonaSunShine_TrueGear.BuildInfo.Description)]
[assembly: AssemblyDescription(ArizonaSunShine_TrueGear.BuildInfo.Description)]
[assembly: AssemblyCompany(ArizonaSunShine_TrueGear.BuildInfo.Company)]
[assembly: AssemblyProduct(ArizonaSunShine_TrueGear.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + ArizonaSunShine_TrueGear.BuildInfo.Author)]
[assembly: AssemblyTrademark(ArizonaSunShine_TrueGear.BuildInfo.Company)]
[assembly: AssemblyVersion(ArizonaSunShine_TrueGear.BuildInfo.Version)]
[assembly: AssemblyFileVersion(ArizonaSunShine_TrueGear.BuildInfo.Version)]
[assembly: MelonInfo(typeof(ArizonaSunShine_TrueGear.ArizonaSunShine_TrueGear), ArizonaSunShine_TrueGear.BuildInfo.Name, ArizonaSunShine_TrueGear.BuildInfo.Version, ArizonaSunShine_TrueGear.BuildInfo.Author, ArizonaSunShine_TrueGear.BuildInfo.DownloadLink)]
[assembly: MelonColor()]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]