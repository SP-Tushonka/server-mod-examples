using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Cloners;
using System.Reflection;
using Path = System.IO.Path;

namespace _13._1AddTraderWithDynamicAssorts;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.sp-tarkov.examples.addtraderdynamicassorts";
    public override string Name { get; init; } = "AddTraderWithDynamicAssortsExample";
    public override string Author { get; init; } = "SPTarkov";
    public override List<string>? Contributors { get; init; } = ["Clodan", "CWX"];
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    
    
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; } = "https://github.com/sp-tarkov/server-mod-examples";
    public override bool? IsBundleMod { get; init; } = false;
    public override string License { get; init; } = "MIT";
}

// This line tells the class to load right after "PostDBModLoader" occurs
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class AddTraderWithDynamicAssorts(
    ISptLogger<AddTraderWithDynamicAssorts> logger,
    ModHelper modHelper,
    DatabaseService databaseService,
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

    public Task OnLoad()
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
        var mp133PresetItems = cloner.Clone(databaseService.GetTables().Globals.ItemPresets.GetValueOrDefault(mp133PresetId).Items);
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
