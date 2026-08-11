using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _14AfterDBLoadHook;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp-tarkov.examples.afterdbhook";
    public string Name { get; init; } = "AfterDBLoadHookExample";
    public string Author { get; init; } = "SPTarkov";
    public List<string>? Contributors { get; init; }
    public SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    
    
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; }
    public string License { get; init; } = "MIT";
    public bool HasPrepatcher { get; init; } = false;
}

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class AfterDBLoadHook(
    TemplateTable templateTable,
    ISptLogger<AfterDBLoadHook> logger) : IOnLoad
{
    private Dictionary<MongoId, TemplateItem>? _itemsDb;

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        _itemsDb = templateTable.Items;

        // Database will be loaded, this is the fresh state of the DB so NOTHING from the SPT
        // logic has modified anything yet. This is the DB loaded straight from the JSON files
        logger.LogWithColor($"Database item size: {_itemsDb.Count}", Spectre.Console.Color.Red, Spectre.Console.Color.Yellow);

        // lets do a quick modification and see how this looks later on
        // find the nvgs item by its Id
        // this also checks if the item exists before giving you the item
        // if it doesn't, this if check will fail
        if (_itemsDb.TryGetValue(ItemTpl.NIGHTVISION_L3HARRIS_GPNVG18_NIGHT_VISION_GOGGLES, out var nvgs))
        {
            // Lets log the state before the modification
            logger.LogWithColor($"NVGs default CanSellOnRagfair: {nvgs.Properties.CanSellOnRagfair}", Spectre.Console.Color.Red,
                Spectre.Console.Color.Yellow);

            // Update one of its properties to be true
            nvgs.Properties.CanSellOnRagfair = true;
        }

        return Task.CompletedTask;
    }
}

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class AfterSptLoadHook(
    TemplateTable templateTable,
    ISptLogger<AfterDBLoadHook> logger) : IOnLoad
{

    private Dictionary<MongoId, TemplateItem>? _itemsDb;

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        _itemsDb = templateTable.Items;

        // The modification we made above would have been processed by now by SPT, so any values we changed had
        // already been passed through the initial lifecycles (OnLoad) of SPT.

        if (_itemsDb.TryGetValue(ItemTpl.NIGHTVISION_L3HARRIS_GPNVG18_NIGHT_VISION_GOGGLES, out var nvgs))
        {
            // Lets log the state after the modification
            logger.LogWithColor($"NVGs default CanSellOnRagfair: {nvgs.Properties.CanSellOnRagfair}",
                Spectre.Console.Color.Red, Spectre.Console.Color.Yellow);
        }

        return Task.CompletedTask;
    }
}

