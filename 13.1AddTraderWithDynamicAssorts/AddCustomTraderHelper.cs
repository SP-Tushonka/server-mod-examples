using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Utils.Cloners;

namespace _13._1AddTraderWithDynamicAssorts
{
    /// <summary>
    /// We inject this class into 'AddTraderWithDynamicAssorts' to help us with adding the new trader into the server
    /// </summary>
    [Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
    public class AddCustomTraderHelper(
        ICloner cloner,
        TradersTable tradersTable,
        LocaleTable localeTable)
    {
        /// <summary>
        /// Add the traders update time for when their offers refresh
        /// </summary>
        /// <param name="traderConfig">trader config to add our trader to</param>
        /// <param name="baseJson">json file for trader (db/base.json)</param>
        /// <param name="refreshTimeSecondsMin">How many seconds between trader stock refresh min time</param>
        /// <param name="refreshTimeSecondsMax">How many seconds between trader stock refresh max time</param>
        public void SetTraderUpdateTime(TraderConfig traderConfig, TraderBase baseJson, int refreshTimeSecondsMin, int refreshTimeSecondsMax)
        {
            // Add refresh time in seconds to config
            var traderRefreshRecord = new UpdateTime
            {
                TraderId = baseJson.Id,
                Seconds = new MinMax<int>(refreshTimeSecondsMin, refreshTimeSecondsMax)
            };

            traderConfig.UpdateTime.Add(traderRefreshRecord);
        }

        /// <summary>
        /// Add a traders base data to the server, no assort items
        /// </summary>
        /// <param name="traderDetailsToAdd">trader details</param>
        public void AddTraderWithEmptyAssortToDb(TraderBase traderDetailsToAdd)
        {
            // Create an empty assort ready for our items
            var emptyTraderItemAssortObject = new TraderAssort
            {
                Items = [],
                BarterScheme = new Dictionary<MongoId, List<List<BarterScheme>>>(),
                LoyalLevelItems = new Dictionary<MongoId, int>()
            };

            // Create trader data ready to add to database
            var traderDataToAdd = new Trader
            {
                Assort = emptyTraderItemAssortObject,
                Base = cloner.Clone(traderDetailsToAdd),
                QuestAssort = new() // quest assort is empty as trader has no assorts unlocked by quests
                {
                    { "Started", new() },
                    { "Success", new() },
                    { "Fail", new() }
                },
                Dialogue = []
            };

            // Add the new trader id and data to the server
            if (!tradersTable.TryAdd(traderDetailsToAdd.Id, traderDataToAdd))
            {
                //Failed to add trader!
            }
        }

        /// <summary>
        /// Add traders name/location/description to all locales (e.g. German/French/English)
        /// </summary>
        /// <param name="baseJson">json file for trader (db/base.json)</param>
        /// <param name="firstName">First name of trader</param>
        /// <param name="description">Flavor text of whom the trader is</param>
        public void AddTraderToLocales(TraderBase baseJson, string firstName, string description)
        {
            // For each language, add locale for the new trader
            var locales = localeTable.Global;
            var newTraderId = baseJson.Id;
            var fullName = baseJson.Name;
            var nickName = baseJson.Nickname;
            var location = baseJson.Location;

            foreach (var (localeKey, localeKvP) in locales)
            {
                // We have to add a transformer here, because locales are lazy loaded due to them taking up huge space in memory
                // The transformer will make sure that each time the locales are requested, the ones added below are included
                localeKvP.AddTransformer(lazyloadedLocaleData =>
                {
                    lazyloadedLocaleData.Add($"{newTraderId} FullName", fullName);
                    lazyloadedLocaleData.Add($"{newTraderId} FirstName", firstName);
                    lazyloadedLocaleData.Add($"{newTraderId} Nickname", nickName);
                    lazyloadedLocaleData.Add($"{newTraderId} Location", location);
                    lazyloadedLocaleData.Add($"{newTraderId} Description", description);
                    return lazyloadedLocaleData;
                });
            }
        }

        /// <summary>
        /// Create a complete weapon from scratch.
        /// Weapons start with a 'root' item
        /// They there have various 'child' items that attach off of the root, the discord mod support can help direct you on how to figure our what you need
        /// </summary>
        /// <returns>A complete glock</returns>
        public List<Item> CreateGlock()
        {
            // Create an array ready to hold the glock and all its mods
            var glock = new List<Item>();

            // Add the base (root) first
            glock.Add(new Item
            { // Add the base weapon first
                Id =
                NewItemIds.GLOCK_BASE, // Ids matter, Ids MUST be unique for every item
                Template = new MongoId("5a7ae0c351dfba0017554310")
                , // This is the weapons tpl, found on: https://db.sp-tushonka.com/search
            });

            // Add barrel
            glock.Add(new Item
            {
                Id =
                NewItemIds.GLOCK_BARREL,
                Template = new MongoId("5a6b60158dc32e000a31138b"),
                ParentId =
                NewItemIds.GLOCK_BASE, // This is a sub item, you need to define its parent it is attached to / inserted into
                SlotId =
                "mod_barrel", // Required for mods, you need to define what 'slot' the mod will fill on the weapon
            });

            // Add receiver
            glock.Add(new Item
            {
                Id =
                NewItemIds.GLOCK_RECIEVER,
                Template = new MongoId("5a9685b1a2750c0032157104"),
                ParentId =
                NewItemIds.GLOCK_BASE,
                SlotId =
                "mod_reciever",
            });

            // Add compensator
            glock.Add(new Item
            {
                Id =
                NewItemIds.GLOCK_COMPENSATOR,
                Template =
                new MongoId("5a7b32a2e899ef00135e345a"),
                ParentId =
                NewItemIds.GLOCK_RECIEVER, // The parent of this mod is the receiver NOT weapon, be careful to get the correct parent
                SlotId =
                "mod_muzzle",
            });

            // Add Pistol grip
            glock.Add(new Item
            {
                Id =
                NewItemIds.GLOCK_PISTOL_GRIP,
                Template =
                new MongoId("5a7b4960e899ef197b331a2d"),
                ParentId =
                NewItemIds.GLOCK_BASE,
                SlotId =
                "mod_pistol_grip",
            });

            // Add front sight
            glock.Add(new Item
            {
                Id =
                NewItemIds.GLOCK_FRONT_SIGHT,
                Template =
                new MongoId("5a6f5d528dc32e00094b97d9"),
                ParentId =
                NewItemIds.GLOCK_RECIEVER,
                SlotId =
                "mod_sight_rear",
            });

            // Add rear sight
            glock.Add(new Item
            {
                Id =
                NewItemIds.GLOCK_REAR_SIGHT,
                Template =
                new MongoId("5a6f58f68dc32e000a311390"),
                ParentId =
                NewItemIds.GLOCK_RECIEVER,
                SlotId =
                "mod_sight_front",
            });

            // Add magazine
            glock.Add(new Item
            {
                Id =
                NewItemIds.GLOCK_MAGAZINE,
                Template =
                new MongoId("630769c4962d0247b029dc60"),
                ParentId =
                NewItemIds.GLOCK_BASE,
                SlotId =
                "mod_magazine",
            });

            return glock;
        }
    }
}
