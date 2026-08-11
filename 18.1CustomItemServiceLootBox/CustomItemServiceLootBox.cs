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

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp-tarkov.examples.customitemlootbox";
    public string Name { get; init; } = "CustomItemServiceLootBoxExample";
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

// Inject just after the database has loaded
[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
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
