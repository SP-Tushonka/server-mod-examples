using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Services.Locales;
using SPTarkov.Server.Core.Models.Spt.Config;

namespace _6OverrideMethod;

/// <summary>
/// This is the replacement for the former package.json data. This is required for all mods.
///
/// This is where we define all the metadata associated with this mod.
/// You don't have to do anything with it, other than fill it out.
/// All properties must be overriden, properties you don't use may be left null.
/// It is read by the mod loader when this mod is loaded.
/// </summary>
public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp-tarkov.examples.overridemethod";
    public string Name { get; init; } = "OverrideMethodExample";
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

[Injectable(TypePriority = OnLoadOrder.Watermark)] // The same load order value needs to be used as the overridden methods containing type
public class OverrideMethod(
    ISptLogger<Watermark> logger, // The logger needs to use the same type as the overridden type (in this case, Watermark)
    ServerLocalisationService localisationService,
    CoreConfig coreConfig,
    WatermarkLocale watermarkLocale)
    : Watermark(logger, localisationService, watermarkLocale, coreConfig) // You must provide the parameters the overridden type requires
{

    public new async Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // We add a log message to the init method
        logger.Success("This is a watermark mod override!");
    
        // perform any asynchronous operations here, using await
    
        // This runs the original method (optional)
        await base.OnLoadAsync(cancellationToken);
    }
}
