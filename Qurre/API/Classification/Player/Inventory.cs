using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Qurre.API.Controllers;
using System.Linq;
using System.Collections.Generic;

namespace Qurre.API.Classification.Player
{
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using Qurre.API;
    using Qurre.API.Addons.Items;
    using Qurre.API.Classification.Structs;

    public sealed class Inventory
    {
        public InventorySystem.Inventory Base { get; }
        public AmmoBox Ammo { get; }

        public int ItemsCount => Base.UserInventory.Items.Count;

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

        private readonly Player _player;

        internal Inventory(Player player)
        {
            Base = player.ReferenceHub.inventory;
            Ammo = new(player);
            _player = player;
        }

        public bool HasItem(ItemType item)
            => Base.UserInventory.Items.Any(tempItem => tempItem.Value.ItemTypeId == item);

        public void Reset(IEnumerable<Item> newItems)
        {
            Clear();

            foreach (Item item in newItems)
            {
                AddItem(item);
            }
        }
        public void Reset(IEnumerable<ItemType> newItems)
        {
            Clear();

            foreach (ItemType type in newItems)
            {
                AddItem(type);
            }
        }

        public void Clear()
        {
            while (Base.UserInventory.Items.Count > 0)
                Base.ServerRemoveItem(Base.UserInventory.Items.ElementAt(0).Key, null);
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

        public Item AddItem(ItemBase itemBase)
        {
            if (itemBase == null)
                return null;

            if (itemBase?.PickupDropModel == null)
                return null;

            Base.UserInventory.Items[itemBase.PickupDropModel.NetworkInfo.Serial] = itemBase;

            itemBase.OnAdded(itemBase.PickupDropModel);
            if (itemBase is Firearm)
                AttachmentsServerHandler.SetupProvidedWeapon(_player.ReferenceHub, itemBase);

            Base.SendItemsNextFrame = true;

            return Item.Get(itemBase);
        }

        public void AddItem(Item item)
        {
            if (item == null)
                return;

            AddItem(item.Base);
        }

        public void AddItem(Item item, int amount)
        {
            if (item == null || amount < 0)
                return;

            for (int i = 0; i < amount; i++)
            {
                AddItem(item.Base);
            }
        }

        public Item AddItem(ItemType itemType)
        {
            Item item = Item.Get(Base.ServerAddItem(itemType));
            if (item is Gun gun)
            {
                if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(_player.ReferenceHub, out Dictionary<ItemType, uint> _d)
                    && _d.TryGetValue(itemType, out uint _y))
                    gun.Base.ApplyAttachmentsCode(_y, true);
                FirearmStatusFlags status = FirearmStatusFlags.MagazineInserted;
                if (gun.Base.HasAdvantageFlag(AttachmentDescriptiveAdvantages.Flashlight))
                    status |= FirearmStatusFlags.FlashlightEnabled;
                gun.Base.Status = new FirearmStatus(gun.MaxAmmo, status, gun.Base.GetCurrentAttachmentsCode());
            }
            return item;
        }

        public void AddItem(ItemType itemType, int amount)
        {
            if (amount < 0)
                return;

            for (int i = 0; i < amount; i++)
            {
                AddItem(itemType);
            }
        }

        public void AddItem(IEnumerable<Item> items)
        {
            if (items == null || items.Count() == 0)
                return;

            foreach (Item item in items)
            {
                AddItem(item.Base);
            }
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