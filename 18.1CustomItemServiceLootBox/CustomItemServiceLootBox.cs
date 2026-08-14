using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Services.Modding.Custom;


namespace _18._1CustomItemServiceLootBox;

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
    public string ModGuid { get; init; } = "com.sp-tushonka.examples.customitemlootbox";

    /// <summary>
    /// The display name of the mod.
    /// </summary>
    public string Name { get; init; } = "CustomItemServiceLootBoxExample";

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

// The database loads in preload, we should immediately add or update items here
[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class CustomItemServiceLootBox(
    ISptLogger<CustomItemServiceLootBox> logger,
    TemplateTable templateTable,
    InventoryConfig inventoryConfig,
    CustomItemService customItemService
) : IOnLoad
{
    private Dictionary<MongoId, TemplateItem>? _itemDb;

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        _itemDb = templateTable.Items;

        // Example of adding new item by cloning existing item using createCloneDetails
        const string crateId = "new_crate_with_randomized_content";
        var exampleCloneItem = new NewItemFromCloneDetails
        {
            NewItemName = "exampleCrate",
            // The item we want to clone, in this example i will cloning the sealed weapon crate
            ItemTplToClone = "6489b2b131a2135f0d7d0fcb",
            // ParentId refers to the Node item the container will be under, you can check it in https://db.sp-tushonka.com/search
            ParentId = "62f109593b54472778797866",
            // The new id of our cloned item
            NewId = crateId,
            FleaPriceRoubles = 50000,
            HandbookPriceRoubles = 42500,
            // Handbook Parent Id refers to the category the container will be under
            // Handbook parent can be found in SPT_Data\Server\database\templates.
            HandbookParentId = "62f109593b54472778797866",
            Locales = new Dictionary<string, LocaleDetails>
            {
                {"en", new LocaleDetails
                    {
                        Name = "Custom Lootbox",
                        ShortName = "Custom Lootbox",
                        Description = "A custom lootbox container"
                    }
                }
            },
            OverrideProperties = new TemplateItemProperties
            {
                Name = "Custom Lootbox",
                ShortName = "Custom Lootbox",
                Description = "A custom lootbox container",
                Weight = 15
            },
        };

        // Basically calls the function and tell the server to add our Cloned new item into the server
        customItemService.CreateItemFromClone(exampleCloneItem);

        // Change item _name to remove it from the *actual* sealed weapon crate logic, this removes it from airdrops and allows easier access to change the contents

        var customItemInDb = _itemDb.GetValueOrDefault(crateId);
        customItemInDb.Name = crateId;

        // Add to inventory config with custom item pool
        inventoryConfig.RandomLootContainers[crateId] = new RewardDetails
        {
            RewardCount = 6,
            FoundInRaid = true,
            RewardTplPool = new Dictionary<MongoId, double>
            {
                {new MongoId("57514643245977207f2c2d09"), 1},
                {new MongoId("544fb62a4bdc2dfb738b4568"), 1},
                {new MongoId("57513f07245977207e26a311"), 1},
                {new MongoId("57513f9324597720a7128161"), 1},
                {new MongoId("57513fcc24597720a31c09a6"), 1},
                {new MongoId("5e8f3423fd7471236e6e3b64"), 1},
                {new MongoId("60b0f93284c20f0feb453da7"), 1},
                {new MongoId("5734773724597737fd047c14"), 1},
                {new MongoId("59e3577886f774176a362503"), 1},
                {new MongoId("57505f6224597709a92585a9"), 1},
                {new MongoId("544fb6cc4bdc2d34748b456e"), 1}
            }
        };
        
       return Task.CompletedTask;
    }
}
