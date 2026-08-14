using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Cloners;
using System.Reflection;
using Path = System.IO.Path;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _13._1AddTraderWithDynamicAssorts;

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
    public string ModGuid { get; init; } = "com.sp-tushonka.examples.addtraderdynamicassorts";

    /// <summary>
    /// The display name of the mod.
    /// </summary>
    public string Name { get; init; } = "AddTraderWithDynamicAssortsExample";

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

// This line tells the class to load right after "PostLoad" occurs
[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class AddTraderWithDynamicAssorts(
    ISptLogger<AddTraderWithDynamicAssorts> logger,
    GlobalTable globalTable,
    ModHelper modHelper,
    ImageRouter imageRouter,
    TraderConfig traderConfig,
    RagfairConfig ragfairConfig,
    TimeUtil timeUtil,
    ICloner cloner,
    FluentTraderAssortCreator fluentAssortCreator, // This is a custom class we add for this mod, we made it injectable so it can be accessed like other classes here
    AddCustomTraderHelper addCustomTraderHelper // This is a custom class we add for this mod, we made it injectable so it can be accessed like other classes here
    )
    : IOnLoad
{

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // A path to the mods files we use below
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

        // A relative path to the trader icon to show
        var traderImagePath = Path.Combine(pathToMod, "db/cat.jpg");

        // The base json containing trader settings we will add to the server
        var traderBase = modHelper.GetJsonDataFromFile<TraderBase>(pathToMod, "db/base.json");

        // Create a helper class and use it to register our traders image/icon + set its stock refresh time
        imageRouter.AddRoute(traderBase.Avatar.Replace(".jpg", ""), traderImagePath);
        addCustomTraderHelper.SetTraderUpdateTime(traderConfig, traderBase, timeUtil.GetHoursAsSeconds(1), timeUtil.GetHoursAsSeconds(2));

        // Add our trader to the config list, this lets it be seen by the flea market
        ragfairConfig.Traders.TryAdd(traderBase.Id, true);

        // Add our trader (with no items yet) to the server database
        // An 'assort' is the term used to describe the offers a trader sells, it has 3 parts to an assort
        // 1: The item
        // 2: The barter scheme, cost of the item (money or barter)
        // 3: The Loyalty level, what rep level is required to buy the item from trader
        addCustomTraderHelper.AddTraderWithEmptyAssortToDb(traderBase);

        // Add localisation text for our trader to the database so it shows to people playing in different languages
        addCustomTraderHelper.AddTraderToLocales(traderBase, "Cat", "This is the cat shop. Meow.");

        // Add a single "milk" for 2000 roubles that player can buy a max of 10 per refresh
        fluentAssortCreator
            .CreateSingleAssortItem(ItemTpl.DRINK_PACK_OF_MILK)
            .AddStackCount(200)
            .AddBuyRestriction(10)
            .AddMoneyCost(Money.ROUBLES, 2000)
            .AddLoyaltyLevel(1)
            .Export(traderBase.Id);

        // Add a 3x bitcoin + salewa for milk barter
        fluentAssortCreator
            .CreateSingleAssortItem(ItemTpl.DRINK_PACK_OF_MILK)
            .AddStackCount(100)
            .AddBarterCost(ItemTpl.BARTER_PHYSICAL_BITCOIN, 3)
            .AddBarterCost(ItemTpl.MEDKIT_SALEWA_FIRST_AID_KIT, 1)
            .AddLoyaltyLevel(1)
            .Export(traderBase.Id);


        // Add glock as a rouble purchase
        fluentAssortCreator
            .CreateComplexAssortItem(addCustomTraderHelper.CreateGlock())
            .AddUnlimitedStackCount()
            .AddMoneyCost(Money.ROUBLES, 20000)
            .AddBuyRestriction(3)
            .AddLoyaltyLevel(1)
            .Export(traderBase.Id);

        // Add mp133 preset as a barter for mayonnaise
        // We give it the id of the mp133 weapon preset found in globals.json
        // Most weapons have a 'default' in `Globals.ItemPresets`
        var mp133PresetId = "584148f2245977598f1ad387";

        // We need to clone the preset before we use it, if we dont it alters the data in the server and breaks server
        var mp133PresetItems = cloner.Clone(globalTable.ItemPresets.GetValueOrDefault(mp133PresetId).Items);
        fluentAssortCreator
            .CreateComplexAssortItem(mp133PresetItems)
            .AddStackCount(200)
            .AddBarterCost(ItemTpl.FOOD_JAR_OF_DEVILDOG_MAYO, 1)
            .AddBuyRestriction(3)
            .AddLoyaltyLevel(1)
            .Export(traderBase.Id);

        // Happy little log message
        logger.Success("Added Cat trader to server");

        // Send back a success to the server to say our trader is good to go
        return Task.CompletedTask;
    }
}

// These are unique IDs we've generated earlier to save time when adding the glock
public static class NewItemIds
{
    public static string GLOCK_BASE = "66eeef3b2a166b73d2066a74";
    public static string GLOCK_BARREL = "66eeef3b2a166b73d2066a75";
    public static string GLOCK_RECIEVER = "66eeef3b2a166b73d2066a76";
    public static string GLOCK_COMPENSATOR = "66eeef3b2a166b73d2066a77";
    public static string GLOCK_PISTOL_GRIP = "66eeef3b2a166b73d2066a78";
    public static string GLOCK_REAR_SIGHT = "66eeef3b2a166b73d2066a79";
    public static string GLOCK_FRONT_SIGHT = "66eeef3b2a166b73d2066a7a";
    public static string GLOCK_MAGAZINE = "66eeef3b2a166b73d2066a7b";
}
