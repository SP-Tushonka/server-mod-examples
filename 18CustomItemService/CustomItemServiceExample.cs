using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Services.Modding.Custom;
using SPTarkov.Server.Core.Models.Enums;

namespace _18CustomItemService;

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
