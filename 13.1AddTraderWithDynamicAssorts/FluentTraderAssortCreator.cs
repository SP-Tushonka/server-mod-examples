using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace _13._1AddTraderWithDynamicAssorts;

/// <summary>
/// We inject this class into 'AddTraderWithDynamicAssorts' to help us add items to the trader to sell
/// </summary>
[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class FluentTraderAssortCreator(
    TradersTable tradersTable,
    ISptLogger<FluentTraderAssortCreator> logger)
{
    private readonly List<Item> _itemsToSell = [];
    private readonly Dictionary<string, List<List<BarterScheme>>> _barterScheme = [];
    private readonly Dictionary<string, int> _loyaltyLevel = [];

    public FluentTraderAssortCreator CreateSingleAssortItem(MongoId itemTpl, MongoId? itemId = null)
    {
        // Create item ready for insertion into assort table
        var newItemToAdd = new Item
        {
            Id = itemId ?? new MongoId(),
            Template = itemTpl,
            ParentId = "hideout", // Should always be "hideout"
            SlotId = "hideout", // Should always be "hideout"
            Upd = new Upd
            {
                UnlimitedCount = false,
                StackObjectsCount = 100
            }
        };

        _itemsToSell.Add(newItemToAdd);

        return this;
    }

    public FluentTraderAssortCreator CreateComplexAssortItem(List<Item> items)
    {
        items[0].ParentId = "hideout";
        items[0].SlotId = "hideout";

        items[0].Upd ??= new Upd();

        items[0].Upd.UnlimitedCount = false;
        items[0].Upd.StackObjectsCount = 100;

        _itemsToSell.AddRange(items);

        return this;
    }

    public FluentTraderAssortCreator AddStackCount(int stackCount)
    {
        _itemsToSell[0].Upd.StackObjectsCount = stackCount;

        return this;
    }

    public FluentTraderAssortCreator AddUnlimitedStackCount()
    {
        _itemsToSell[0].Upd.StackObjectsCount = 999999;
        _itemsToSell[0].Upd.UnlimitedCount = true;

        return this;
    }

    public FluentTraderAssortCreator MakeStackCountUnlimited()
    {
        _itemsToSell[0].Upd.StackObjectsCount = 999999;

        return this;
    }

    public FluentTraderAssortCreator AddBuyRestriction(int maxBuyLimit)
    {
        _itemsToSell[0].Upd.BuyRestrictionMax = maxBuyLimit;
        _itemsToSell[0].Upd.BuyRestrictionCurrent = 0;

        return this;
    }

    public FluentTraderAssortCreator AddLoyaltyLevel(int level)
    {
        _loyaltyLevel[_itemsToSell[0].Id] = level;

        return this;
    }

    public FluentTraderAssortCreator AddMoneyCost(string currencyType, int amount)
    {
        var dataToAdd = new BarterScheme
        {
            Count = amount,
            Template = currencyType
        };

        if (!_barterScheme.TryAdd(_itemsToSell[0].Id, [[dataToAdd]]))
        {
            logger.Warning($"Unable to add barter scheme currency: {currencyType}");
        }

        return this;
    }

    public FluentTraderAssortCreator AddBarterCost(MongoId itemTpl, int count)
    {
        var sellableItemId = _itemsToSell[0].Id;

        // No data at all, create
        if (_barterScheme.Count == 0)
        {
            var dataToAdd = new BarterScheme
            {
                Count = count,
                Template = itemTpl
            };

            _barterScheme[sellableItemId] = [[dataToAdd]];
        }
        else
        {
            // Item already exists, add to
            var existingData = _barterScheme[sellableItemId][0].FirstOrDefault(x => x.Template == itemTpl);
            if (existingData is not null)
            {
                // itemTpl already a barter for item, add to count
                existingData.Count += count;
            }
            else
            {
                // No barter for item, add it fresh
                _barterScheme[sellableItemId][0].Add(new BarterScheme
                {
                    Count = count,
                    Template = itemTpl
                });
            }
        }

        return this;
    }

    /// <summary>
    /// Store the generated assort in the server db against the desired trader
    /// </summary>
    /// <param name="traderId">Id of trader to add assort to</param>
    public FluentTraderAssortCreator? Export(string traderId)
    {
        var traderData = tradersTable.GetValueOrDefault(traderId);

        var rootItemAddedId = _itemsToSell.FirstOrDefault().Id;
        if (traderData.Assort.Items.Exists(x => x.Id == rootItemAddedId))
        {
            logger.Error($"Unable to add complex item with item key: {_itemsToSell[0].Id}, key already in use");

            _itemsToSell.Clear();
            _barterScheme.Clear();
            _loyaltyLevel.Clear();

            return null;
        }

        traderData.Assort.Items.AddRange(_itemsToSell);
        traderData.Assort.BarterScheme[rootItemAddedId] = _barterScheme[rootItemAddedId];
        traderData.Assort.LoyalLevelItems[rootItemAddedId] = _loyaltyLevel[rootItemAddedId];

        _itemsToSell.Clear();
        _barterScheme.Clear();
        _loyaltyLevel.Clear();

        return this;
    }
}
