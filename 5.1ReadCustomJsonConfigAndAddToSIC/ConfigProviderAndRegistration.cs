using System.Reflection;
using System.Text.Json;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Web.Models.Configs;
using SPTarkov.Server.Web.Services;

namespace _5._1ReadCustomJsonConfigAndAddToSIC;

// We need to use the ConfigRegistration so every class in our mod can take our ModConfig as a constructor parameter through DI
public class ConfigRegistration : IOnDIConstruct
{
    public static async Task OnDIConstructAsync(IServiceCollection serviceCollection, CancellationToken ct)
    {
        ModConfig config = await LoadConfigFromDiskAsync(ct);
        serviceCollection.AddSingleton(config);
    }

    private static async Task<ModConfig> LoadConfigFromDiskAsync(CancellationToken ct)
    {
        var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "config.json");

        if (!File.Exists(configPath))
        {
            var defaultConfig = new ModConfig
            {
                // If you return new ModConfig() here, the properties on ModConfig must have usable default values. Required properties must also be initialized.
                ExampleProperty = "boop"
            };
            await SaveConfigToDiskAsync(defaultConfig, configPath, ct);
            return defaultConfig;
        }

        await using FileStream stream = File.OpenRead(configPath);
        var config = await JsonSerializer.DeserializeAsync<ModConfig>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);

        return config ?? new ModConfig
        {
            // We're checking and setting here as well incase the config did not deserialize, that way we don't throw errors.
            // You can still, instead, use default values on your ModConfig
            ExampleProperty = "boop"
        };
    }

    private static async Task SaveConfigToDiskAsync(ModConfig config, string path, CancellationToken ct)
    {
        await using FileStream stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, config, new JsonSerializerOptions { WriteIndented = true }, ct);
    }
}

// Create the Config Provider as a Singleton
[Injectable(InjectionType.Singleton)]
public class ConfigProvider(
    ModConfig config // Because we registered our mod config we can now inject it into our classes
    ) : IConfigEditorConfigProvider
{
    public IEnumerable<ConfigEditorConfigRegistration> GetConfigs()
    {
        // Use your mod metadata for the SIC registration ID and display name. Make sure the file path points to your mod's actual config file.
        var metadata = new ModMetadata();
        yield return ConfigEditorConfigRegistration.Create(
            metadata.ModGuid,
            metadata.Name,
            config,
            Path.Combine("user", "mods", "5.1ReadCustomJsonConfigAndAddToSIC", "config.json")
            );
    }
}