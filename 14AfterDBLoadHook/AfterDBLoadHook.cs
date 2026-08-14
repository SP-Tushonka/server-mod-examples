using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _14AfterDBLoadHook;

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
    public string ModGuid { get; init; } = "com.sp-tushonka.examples.afterdbhook";

    /// <summary>
    /// The display name of the mod.
    /// </summary>
    public string Name { get; init; } = "AfterDBLoadHookExample";

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
            logger.LogWithColor($"NVGs default CanSellOnRagfair: {nvgs.Properties.CanSellOnRagfair}", 
                Spectre.Console.Color.Red, Spectre.Console.Color.Yellow);

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

