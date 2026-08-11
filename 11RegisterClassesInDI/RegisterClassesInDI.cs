using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;

namespace _11RegisterClassesInDI;

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
    public string ModGuid { get; init; } = "com.sp-tarkov.examples.registerclassesindi";
    public string Name { get; init; } = "RegisterClassesInDIExample";
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

// We want to load after PostLoad is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class RegisterClassesInDi(
    SingletonClassExample singletonClassExample,
    ScopedClassExample scopedClassExample)
    : IOnLoad
{
    // We inject 2 classes (singleton and scoped) we've made below

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        singletonClassExample.IncrementCounterAndLog();
        singletonClassExample.IncrementCounterAndLog();
        singletonClassExample.IncrementCounterAndLog();

        scopedClassExample.IncrementCounterAndLog();
        scopedClassExample.IncrementCounterAndLog();
        scopedClassExample.IncrementCounterAndLog();
        
        return Task.CompletedTask;
    }
}

// This class is registered as a singleton. This means ONE and only ONE instance
// of this class will ever exist.
[Injectable(InjectionType.Singleton)]
public class SingletonClassExample(ISptLogger<SingletonClassExample> logger)
{
    private int _counter = 0;

    public void IncrementCounterAndLog()
    {
        _counter++;
        logger.Success($"{_counter}");
    }
}

// This class is being registered as default or scoped. This means that
// every time a class requests an instance of this type a new one will be created
[Injectable(InjectionType.Scoped)] // [Injectable] is the same as doing this
public class ScopedClassExample(
    ISptLogger<ScopedClassExample> logger)
{
    private int _counter = 0;

    public void IncrementCounterAndLog()
    {
        _counter++;
        logger.Success($"{_counter}");
    }
}
