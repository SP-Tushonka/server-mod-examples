using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Common.Models.Logging;
using System.Reflection;
using SPTarkov.Server.Core.Helpers.Server;

namespace _5ReadCustomJsonConfig;

// We want to load after Preload is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class ReadJsonConfig(
        ISptLogger<ReadJsonConfig> logger,
        ModHelper modHelper) : IOnLoad // Implement the IOnLoad interface so that this mod can do something
{
    /// <summary>
    /// This is called when this class is loaded, the order in which its loaded is set according to the type priority
    /// on the [Injectable] attribute on this class. Each class can then be used as an entry point to do
    /// things at varying times according to type priority
    /// </summary>
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // This will get us the full path to the mod, e.g. C:\spt\user\mods\5ReadCustomJsonConfig-0.0.1
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

        // We give the path to the mod folder and the file we want to get, giving us the config, supply the files 'type' between the diamond brackets
        var config = modHelper.GetJsonDataFromFile<ModConfig>(pathToMod, "config.json");

        logger.Success($"Read property: 'ExampleProperty' from config with value: {config.ExampleProperty}");

        // Return a completed task
        return Task.CompletedTask;
    }
}

// This class should represent your config structure
public record ModConfig
{
    public required string ExampleProperty { get; set; }
}
