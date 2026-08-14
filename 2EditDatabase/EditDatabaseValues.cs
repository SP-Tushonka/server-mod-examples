using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Enums.Hideout;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _2EditDatabase;

using SPTarkov.Server.Core.Models.Common;

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
    public string ModGuid { get; init; } = "com.sp-tushonka.examples.editdatabase";

    /// <summary>
    /// The display name of the mod.
    /// </summary>
    public string Name { get; init; } = "EditDatabaseExample";

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
public class EditDatabaseValues(
    ISptLogger<EditDatabaseValues> logger,
    GlobalTable globalTable,
    BotTable botTable,
    LocationTable locationTable,
    HideoutTable hideoutTable) // We are injecting a logger similar to example 1, but notice the class inside < > is different
    : IOnLoad // Implement the `IOnLoad` interface so that this mod can do something
{
    // Our constructor

    /// <summary>
    /// This is called when this class is loaded, the order in which its loaded is set according to the type priority
    /// on the [Injectable] attribute on this class. Each class can then be used as an entry point to do
    /// things at varying times according to type priority
    /// </summary>
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // When SPT starts, it stores all the data found in (SPT_Data\Server\database) in memory
        // We can use the 'databaseService' we injected to access this data, this includes files from EFT and SPT

        // Lets edit some globals settings to make the game easier
        // This is a method, a chunk of code we run, ctrl+click the method to go to the code, or click it and press f12
        // Methods are not necessary, but they help to compartmentalise code and made it easier to read/navigate
        EditGlobals();

        // Lets edit the BTR to have the christmas-themed `Tarcola` skin
        EditBtr();

        // Let's edit the hideout so it's easier to upgrade the lavatory
        EditHideout();

        // Lets edit the default scav (assault.json) to have different settings
        EditScavSettings();

        // Lets edit Customs
        EditCustoms();

        // lets write a nice log message to the server console so players know our mod has made changes
        logger.Success("Finished Editing Database!");
        
        // Inform server we have finished
        return Task.CompletedTask;
    }
    
    private void EditGlobals()
    {
        // Let's edit the scav cooldown to be 1 second
        globalTable.Configuration.SavagePlayCooldown = 1;

        // Now lets try editing the ragfair unlock level, lets get the ragfair settings first
        var ragfairSettings = globalTable.Configuration.RagFair;

        // Lets set the level you need to be to access flea to be 1
        ragfairSettings.MinUserLevel = 1;

        // Now lets increase the number of offers you can have listed at one time
        // The max is stored in a list, different flea ratings give different offer amounts

        // We loop over all the settings, setting all of them to be 20
        foreach (var offerCountSettings in ragfairSettings.MaxActiveOfferCount)
        {
            offerCountSettings.Count = 20;
        }
    }

    private void EditBtr()
    {
        // We get the BTR settings from globals first
        var btrSettings = globalTable.Configuration.BTRSettings;

        // Let's get the settings for woods specifically, we use 'tryGetValue' for this, the settings will be stored in 'woodsBtrSettings'
        btrSettings.MapsConfigs.TryGetValue("Woods", out var woodsBtrSettings);

        // Lets set the BTR to use the christmas skin
        woodsBtrSettings?.BtrSkin = "Tarcola";
    }

    private void EditHideout()
    {
        // We want the areas, they're stored in a list
        var hideoutAreas = hideoutTable.Areas;

        // We find the toilet, we use 'firstOrDefault', if we cant find the watercloset, 'waterclosetArea' will be null
        var waterclosetArea = hideoutAreas.FirstOrDefault(area => area.Type == HideoutAreas.WaterCloset);

        if (waterclosetArea == null)
        {
            logger.Error("Oh no, there is no water closet area. Better return early so we don't throw a null reference exception");
            return;
        }

        // Now we have the toilet and we know it isn't null, we can find the requirements to craft, all data is stored by stage
        var toiletStages = waterclosetArea.Stages;

        // Stages are stored in a dictionary, a dictionary has a 'key' and a 'value'
        // In this case, the 'key' is the upgrade stage, e.g. "1", or "2"
        // We reference to each stage as a 'Key value Pair', every key has a value (key = stage number, value = data for that stage)
        // Because `toiletStages` is a nullable dictionary, we can safely loop with a coalescing empty array fallback
        foreach (var (stageKey, stageValue) in toiletStages ?? [])
        {
            // while we're here, we can make the stages craft really fast (60 seconds)
            stageValue.ConstructionTime = 60;

            // Let's get the stage requirements, they're a list
            var stageRequirements = stageValue.Requirements;

            // We empty the requirements out, now it can be built straight away
            stageRequirements?.Clear();
        }
    }

    private void EditScavSettings()
    {
        // Same as the above example, we use 'TryGetValue' to get the 'assault' bot (assault is the internal name for scavs)
        if (botTable.Types.TryGetValue("assault", out var assaultBot))
        {
            // Since BotType is nullable, we might as well check for null and return early if it is. It shouldn't be null, but safer to check because we don't know what other mods are doing.
            if (assaultBot == null)
            {
                return;
            }
            
            // Let's make the chance to get a good backpack really high, but let's do it safely to ensure they have that equipment slot
            if (!assaultBot.BotInventory.Equipment.TryGetValue(EquipmentSlots.Backpack, out var backPacks))
            {
                // The equipment slot wasn't found, so let's create a new dictionary for that slot so we can safely add to it
                backPacks = new Dictionary<MongoId, double>();
            }

            // We access the backpacks dictionary by key directly using square brackets, we use ItemTpl to get the items ID
            // Alternately, we could have typed backPacks["59e763f286f7742ee57895da"] and done the same thing, ItemTpl makes it easier to read
            backPacks.AddOrUpdate(ItemTpl.BACKPACK_PILGRIM_TOURIST, 999999);


            // Now lets make them always have an M4A1 - we'll follow the same safe pattern as above
            if (!assaultBot.BotInventory.Equipment.TryGetValue(EquipmentSlots.FirstPrimaryWeapon, out var primaryWeapons))
            {
                primaryWeapons = new Dictionary<MongoId, double>();
            }

            // We edit the weight value (pick chance) that is already there to be massive, making the item more likely to be picked
            primaryWeapons.AddOrUpdate(ItemTpl.ASSAULTRIFLE_COLT_M4A1_556X45_ASSAULT_RIFLE, 999999);


            // Now lets make them always have the first name of Gary
            // We start by removing all the existing names
            assaultBot.FirstNames.Clear();

            // We add the new name Gary, very menacing
            assaultBot.FirstNames.Add("Gary");
        }
    }

    private void EditCustoms()
    {
        // Customs is called 'bigmap' in eft
        var customs = locationTable.Bigmap;

        // Lets get the exits and make them all 100% chance to appear
        var exits = customs.Base.Exits;

        // They're stored as a list so we can loop over them
        foreach (var exit in exits)
        {
            // I can't remember which one is used, you'd assume ChancePVE is used in pve, but this is BSG we're dealing with
            // So we set both
            exit.Chance = 100;
            exit.ChancePVE = 100;
        }


        // Lets try editing the airdrops on customs to be better
        var airdropSettings = customs.Base.AirdropParameters;

        // They're stored in an array but there's only one bunch of settings, it means we have to get the first item from the list,
        // An alternate way to access the first item is done by using square brackets with the 'index' of the item we want,
        // indexes start at 0 so we want to type "[0]" to access the first item in the list,
        var actualAirdropSettings = airdropSettings.First();

        // Make it spawn 100%
        actualAirdropSettings.PlaneAirdropChance = 1; // Number between 0 and 1

        // Make it spawn as early as start of raid
        actualAirdropSettings.PlaneAirdropStartMin = 1;


        // Let's make bosses spawn 100% of the time

        // We get all the bosses, they're stored in a list
        var bosses = customs.Base.BossLocationSpawn;

        // Let's get Reshala, we use "FirstOrDefault" and look for the first boss with the name "bossBully"
        var reshala = bosses.FirstOrDefault(boss => boss.BossName == "bossBully");

        // Set him to 100%, using conditional access to ensure we found him
        reshala?.BossChance = 100;
    }
}
