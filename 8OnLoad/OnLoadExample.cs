using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;

namespace _8OnLoad;

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
    public string ModGuid { get; init; } = "com.sp-tushonka.examples.onload";
    public string Name { get; init; } = "OnLoadExampleExample";
    public string Author { get; init; } = "SPTDevTeam";
    public List<string>? Contributors { get; init; }
    public SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    
    
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; }
    public string License { get; init; } = "MIT";
    public bool HasPrepatcher { get; init; } = false;
}

// Check `OnLoadOrder` for list of possible choices
[Injectable(TypePriority = OnLoadOrder.PostLoad)] // Can also give an int value for fine-grained control
public class OnLoadExample(
    ISptLogger<OnLoadExample> logger) : IOnLoad // Must implement the IOnLoad interface
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // Can do work here
        logger.Success($"Mod loaded after database!");

        return Task.CompletedTask;
    }
}
