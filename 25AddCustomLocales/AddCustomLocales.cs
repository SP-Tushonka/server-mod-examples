using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Services.Locales;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _25AddCustomLocales;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp-tarkov.examples.customlocales";
    public string Name { get; init; } = "AddCustomLocalesExample";
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
