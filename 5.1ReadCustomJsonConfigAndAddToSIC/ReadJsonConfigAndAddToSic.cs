using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace _5._1ReadCustomJsonConfigAndAddToSIC;

// We want to load after Preload is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class ReadJsonConfigAndAddToSic(
        ISptLogger<ReadJsonConfigAndAddToSic> logger,
        ModConfig modConfig // We can inject our mod config because we are registering it in `ConfigProviderAndRegistration.cs`
        ) : IOnLoad // Implement the IOnLoad interface so that this mod can do something
{
    /// <summary>
    /// This is called when this class is loaded, the order in which its loaded is set according to the type priority
    /// on the [Injectable] attribute on this class. Each class can then be used as an entry point to do
    /// things at varying times according to type priority
    /// </summary>
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        ApplyConfig();

        return Task.CompletedTask;
    }

    public void ApplyConfig()
    {
        logger.Success($"Read property: 'ExampleProperty' from config with value: {modConfig.ExampleProperty}");
    }
}

// This class should represent your config structure, notice we aren't initializing with values
public record ModConfig 
{
    public required string ExampleProperty { get; set; }
}
