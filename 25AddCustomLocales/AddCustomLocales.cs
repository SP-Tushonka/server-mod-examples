using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Services.Locales;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _25AddCustomLocales;

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
    public string ModGuid { get; init; } = "com.sp-tushonka.examples.customlocales";

    /// <summary>
    /// The display name of the mod.
    /// </summary>
    public string Name { get; init; } = "AddCustomLocalesExample";

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
public class AddCustomLocales(
    ISptLogger<AddCustomLocales> logger,
    LocaleService localeService,
    LocaleTable localeTable,
    ServerLocalisationService serverLocalisationService)
    : IOnLoad
{
    // Constructor - Inject a 'ISptLogger' with your mods Class inside the diamond brackets
    // Save the logger we're injecting into a private variable that is scoped to this class (only this class has access to it)
    // save the locale service into a private variable that is scoped to this class (only this class has access to it)

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // Add a custom locale to the en game locales
       if (localeTable.Global.TryGetValue("en", out var lazyloadedValue))
        {
            // We have to add a transformer here, because locales are lazy loaded due to them taking up huge space in memory
            // The transformer will make sure that each time the locales are requested, the ones changed or added below are included
            lazyloadedValue.AddTransformer(lazyloadedLocaleData =>
            {
                lazyloadedLocaleData["Attention! This is a Beta version of Escape from Tarkov for testing purposes."] = "Testing change of beta version warning";
                lazyloadedLocaleData.Add("TestingLocales", "Testing Locales");

                return lazyloadedLocaleData;
            });

            logger.Success("Added a custom locale to the database");
        }

        var _locales = localeService.GetLocaleDb("en");
        // Log this so we can see it in the console
        logger.Info(_locales["TestingLocales"]);

        // Log by the locale key and output the language the player has set
        // If the locale isn't found, it tries english
        // If english isn't found, it shows the key
        logger.Info(serverLocalisationService.GetText("TestingLocales"));

        logger.Info(_locales["Attention! This is a Beta version of Escape from Tarkov for testing purposes."]);
        return Task.CompletedTask;
    }
}
