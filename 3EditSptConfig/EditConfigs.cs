using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;

namespace _3EditSptConfig;

/// <summary>
/// This is required for all mods.
/// This is where we define all the metadata associated with this mod.
/// You don't have to do anything with it, other than fill it out.
/// Properties you don't use may be left null.
/// It is read by the mod loader when this mod is loaded.
/// </summary>
public record ModMetadata : IModMetadata
{
    /// <summary>
    /// A unique identifier for the mod.
    /// Reverse domain notation is recommended to reduce the chance of conflicts with other mods,
    /// for example "com.example.mymod". Avoid generic identifiers such as "mymod" or "mod1".
    /// </summary>
    public string ModGuid { get; init; } = "com.sp-tushonka.examples.editsptconfig";

    /// <summary>
    /// The display name of the mod.
    /// </summary>
    public string Name { get; init; } = "EditConfigsExample";

    /// <summary>
    /// The primary author or maintainer of the mod.
    /// </summary>
    public string Author { get; init; } = "SPTDevTeam";

    /// <summary>
    /// A list of additional contributors who worked on the mod.
    /// Leave null when there are no additional contributors.
    /// </summary>
    public List<string>? Contributors { get; init; }

    /// <summary>
    /// The current version of the mod using Semantic Versioning (SemVer).
    /// Versions follow the MAJOR.MINOR.PATCH format:
    /// MAJOR for breaking changes, MINOR for backwards-compatible features,
    /// and PATCH for backwards-compatible bug fixes.
    /// </summary>
    public SemanticVersioning.Version Version { get; init; } = new("1.0.0");

    /// <summary>
    /// The range of SPT versions supported by this mod.
    /// Use Semantic Versioning range syntax to specify which SPT versions are compatible.
    /// For example, "~4.1.0" allows compatible SPT 4.1.x releases.
    /// The minimum version must match the version of the SPT NuGet package referenced by the mod.
    /// For example, if the mod references SPT NuGet version 4.1.2, use "~4.1.2".
    /// </summary>
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.2");

    /// <summary>
    /// A list of mod GUIDs that are incompatible with this mod.
    /// Leave null when the mod has no known incompatibilities.
    /// </summary>
    public List<string>? Incompatibilities { get; init; }

    /// <summary>
    /// A collection of mod GUIDs and the Semantic Versioning ranges of their required versions.
    /// Use this to declare mods that must be installed for this mod to function correctly.
    /// Leave null when the mod has no dependencies.
    /// </summary>
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }

    /// <summary>
    /// An optional URL where users can find more information about the mod,
    /// such as its documentation, source code, or download page.
    /// Leave null when no URL is available.
    /// </summary>
    public string? Url { get; init; } = "https://github.com/sp-tushonka/server-mod-examples";

    /// <summary>
    /// The license under which the mod is distributed.
    /// For example, "MIT", "GPL-3.0", or another applicable license identifier.
    /// </summary>
    public string License { get; init; } = "MIT";

    /// <summary>
    /// Indicates whether the mod uses Prepatcher.
    /// Set to true if the mod contains Prepatcher patches; otherwise leave false.
    /// </summary>
    public bool HasPrepatcher { get; init; } = false;
}

// We want to load after PostDBModLoader is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class EditConfigs(
    BotConfig botConfig,
    HideoutConfig hideoutConfig,
    WeatherConfig weatherConfig,
    AirdropConfig airdropConfig,
    PmcChatResponseConfig pmcChatResponseConfig,
    QuestConfig questConfig,
    PmcConfig pmcConfig,
    ISptLogger<EditConfigs> logger
) : IOnLoad // Implement the IOnLoad interface so that this mod can do something
{
    // We get a config by injecting it into the class


    /// <summary>
    /// This is called when this class is loaded, the order in which its loaded is set according to the type priority
    /// on the [Injectable] attribute on this class. Each class can then be used as an entry point to do
    /// things at varying times according to type priority
    /// </summary>
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // Let's edit the weather config to force the season to winter
        weatherConfig.OverrideSeason = Season.WINTER;

        // Let's edit the hideout config to Make all crafts take 60 seconds
        hideoutConfig.OverrideCraftTimeSeconds = 60;

        // Let's edit the hideout config to Make all upgrades take 60 seconds
        hideoutConfig.OverrideBuildTimeSeconds = 60;

        // Let's edit the airdrop config to Make weapon/armor drops REALLY common
        // We can use the helper `AddOrUpdate`
        airdropConfig.AirdropTypeWeightings.AddOrUpdate(SptAirdropTypeEnum.weaponArmor, 999);

        // Let's edit the airdrop config to Make weapon/armor drops always have 3 sealed weapon crates
        // When accessing a dictionary, 'TryGetValue' is a safe way to do it, it will return true if it finds the key you want, or false if it doesn't
        // The second parameter 'weaponAndArmorLootSettingsAirdropLoot' is an 'out' parameter, it will be hydrated with the data we want if it's found
        // The examples below that access dictionaries will be the 'unsafe/old' way using square [] brackets. Both approaches will work, you should consider both and consider which suits your needs for your mod
        if (airdropConfig.Loot.TryGetValue("weaponArmor", out var weaponAndArmorLootSettingsAirdropLoot))
        {
            // We found what we wanted in the dictionary, lets make changes
            // Weapon/armor crates will always have 3 sealed weapon crates inside them
            weaponAndArmorLootSettingsAirdropLoot.WeaponCrateCount.Min = 3;
            weaponAndArmorLootSettingsAirdropLoot.WeaponCrateCount.Max = 3;
        }

        // Let's make PMCs always mail you when they kill you
        pmcChatResponseConfig.Killer.ResponseChancePercent = 100;

        // Let's make quest rewards sent to you via mail last for over a week for unheard profiles
        questConfig.MailRedeemTimeHours.AddOrUpdate("unheard_edition", 168);

        // Let's make the interchange bot cap huge
        botConfig.MaxBotCap.AddOrUpdate("interchange", 50);

        // Let's disable loot on scavs
        botConfig.DisableLootOnBotTypes.Add("assault");

        // Lets make PMCs carry absurdly expensive loot in their pockets
        pmcConfig.LootSettings.Pocket.TotalRubByLevel =
        [
            new MinMaxLootValue
            {
                Min = 1,
                Max = 99,
                Value = 9999999
            }
        ];

        logger.Success("Finished Editing Configs");

        // Return a completed task
        return Task.CompletedTask;
    }
}
