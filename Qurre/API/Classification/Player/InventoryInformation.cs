using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Qurre.API.Controllers;
using System.Linq;
using System.Collections.Generic;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;

    public sealed class InventoryInformation
    {
        public Inventory Base { get; }

        public Dictionary<ushort, Item> Items
        {
            get
            {
                return (Dictionary<ushort, Item>)Base.UserInventory.Items.Select(item => new KeyValuePair<ushort, Item>(item.Key, Item.Get(item.Value)));
            }
            set
            {
                if (value == null)
                {
                    Clear();
                    return;
                }

                Base.UserInventory.Items = (Dictionary<ushort, ItemBase>)value.Select(item => new KeyValuePair<ushort, ItemBase>(item.Key, item.Value.Base));
                Base.SendItemsNextFrame = true;
            }
        }

        private readonly GamePlay _gamePlay;

        internal InventoryInformation(Player player)
        {
            Base = player.ReferenceHub.inventory;
            _gamePlay = player.GamePlay;
        }

        public void Reset(IEnumerable<Item> newItems)
        {
            Clear();

            foreach (Item item in newItems)
            {
                AddItem(item);
            }    
        }

        public void Clear()
        {
            _gamePlay.ClearInventory();
        }

        public void DropAllAmmo()
        {
            foreach (KeyValuePair<ItemType, ushort> item in Base.UserInventory.ReserveAmmo)
            {
                if (item.Value > 0)
                {
                    Base.ServerDropAmmo(item.Key, ushort.MaxValue);
                }
            }
        }

        public void DropAllItems()
        {
            while (Base.UserInventory.Items.Count > 0)
            {
                Base.ServerDropItem(Base.UserInventory.Items.First().Key);
            }
        }

        public void DropAll()
        {
            Base.ServerDropEverything();
        }

        public void SelectItem(ushort serial)
        {
            Base.ServerSelectItem(serial);
        }

        public void SelectItem(Item item)
        {
            if (item == null)
                return;

            SelectItem(item.Serial);
        }

        public void DropItem(ushort serial)
        {
            Base.ServerDropItem(serial);
        }

        public void DropItem(Item item)
        {
            if (item == null)
                return;

            Base.ServerDropItem(item.Serial);
        }

        public void AddItem(ItemBase itemBase)
        {
            if (itemBase == null)
                return;

            _gamePlay.AddItem(itemBase);
        }

        public void AddItem(Item item)
        {
            if (item == null)
                return;

            _gamePlay.AddItem(item.Base);
        }

        public void AddItem(Item item, int amount)
        {
            if (item == null)
                return;

            _gamePlay.AddItem(item, amount);
        }

        public void AddItem(ItemType itemType)
        {
            _gamePlay.AddItem(itemType);
        }

        public void AddItem(IEnumerable<Item> items)
        {
            _gamePlay.AddItem(items);
        }

        public void RemoveItem(ushort serial, ItemPickupBase itemPickupBase)
        {
            Base.ServerRemoveItem(serial, itemPickupBase);
        }

        public void RemoveItem(Pickup pickup)
        {
            if (pickup == null)
                return;

            Base.ServerRemoveItem(pickup.Serial, pickup.Base);
        }

        public void RemoveItem(Item item)
        {
            if (item?.Pickup == null)
                return;

            Base.ServerRemoveItem(item.Serial, item.Pickup.Base);
        }
    }
}