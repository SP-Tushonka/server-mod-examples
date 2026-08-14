using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Common.Models.Logging;

namespace _3EditSptConfig;

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
