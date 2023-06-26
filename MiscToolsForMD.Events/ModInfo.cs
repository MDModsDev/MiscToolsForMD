using MelonLoader;
using MiscToolsForMD.SDK;

[assembly: MelonInfo(typeof(Events), "MiscToolsForMD.Events", "0.1.0", "MiscToolsForMD")]
[assembly: MelonGame("PeroPeroGames", "MuseDash")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
// Using MelonLoader v0.6.1 at least
[assembly: VerifyLoaderVersion("0.6.1", true)]
// Supports game v3.4.0
[assembly: MelonGameVersion("3.4.0")]

// This mod does nothing but provides some events for downstream using
