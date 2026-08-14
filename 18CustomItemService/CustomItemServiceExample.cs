using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Services.Modding.Custom;
using SPTarkov.Server.Core.Models.Enums;

namespace _18CustomItemService;

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
    public string ModGuid { get; init; } = "com.sp-tushonka.examples.customitem";

    /// <summary>
    /// The display name of the mod.
    /// </summary>
    public string Name { get; init; } = "CustomItemServiceExample";

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

[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class CustomItemServiceExample(
    ISptLogger<CustomItemServiceExample> logger,
    CustomItemService customItemService) : IOnLoad
{

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        //Example of adding new item by cloning an existing item using `createCloneDetails`
        var exampleCloneItem = new NewItemFromCloneDetails
        {
            NewItemName = string.Empty,
            ItemTplToClone = ItemTpl.SHOTGUN_MP18_762X54R_SINGLESHOT_RIFLE,
            // ParentId refers to the Node item the gun will be under, you can check it in https://db.sp-tushonka.com/search
            ParentId = "5447b6094bdc2dc3278b4567",
            // The new id of our cloned item - MUST be a valid mongo id, search online for mongo id generators
            NewId = "677eed5f2e040616bc7246b6",
            // Flea price of item
            FleaPriceRoubles = 50000,
            // Price of item in handbook
            HandbookPriceRoubles = 42500,
            // Handbook Parent Id refers to the category the gun will be under
            HandbookParentId = "5b5f78e986f77447ed5636b1",
            //you see those side box tab thing that only select gun under specific icon? Handbook parent can be found in Spt_Data\Server\database\templates.
            Locales = new Dictionary<string, LocaleDetails>
            {
                {
                    "en", new LocaleDetails
                    {
                        Name = "MP-18 12g",
                        ShortName = "Custom MP18",
                        Description = "A custom MP18 chambered in 12G"
                    }
                }
            },
            OverrideProperties = new TemplateItemProperties
            {
                Chambers =
                [
                    new Slot
                    {
                        Name = "patron_in_weapon_000",
                        Id = "61f7c9e189e6fb1a5e3ea791",
                        Parent = "CustomMP18",
                        Properties = new SlotProperties
                        {
                            Filters =
                            [
                                new SlotFilter
                                {
                                    Filter =
                                    [
                                        "560d5e524bdc2d25448b4571",
                                        "5d6e6772a4b936088465b17c",
                                        "5d6e67fba4b9361bc73bc779",
                                        "5d6e6806a4b936088465b17e",
                                        "5d6e68dea4b9361bcc29e659",
                                        "5d6e6911a4b9361bd5780d52",
                                        "5c0d591486f7744c505b416f",
                                        "58820d1224597753c90aeb13",
                                        "5d6e68c4a4b9361b93413f79",
                                        "5d6e68a8a4b9360b6c0d54e2",
                                        "5d6e68e6a4b9361c140bcfe0",
                                        "5d6e6869a4b9361c140bcfde",
                                        "5d6e68b3a4b9361bca7e50b5",
                                        "5d6e6891a4b9361bd473feea",
                                        "5d6e689ca4b9361bc8618956",
                                        "5d6e68d1a4b93622fe60e845"
                                    ]
                                }
                            ]
                        },
                        Required = false,
                        MergeSlotWithChildren = false,
                        Prototype = "55d4af244bdc2d962f8b4571"
                    }
                ]
            },
        };

        customItemService.CreateItemFromClone(exampleCloneItem); // Send our data to the function that creates our item
        
        return Task.CompletedTask;
    }
}
