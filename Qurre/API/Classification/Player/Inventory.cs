using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Pickups;
using Qurre.API.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using Qurre.API.Addons.Items;
    using Qurre.API.Classification.Structs;
    using Qurre.API.Objects;

    public sealed class Inventory
    {
        public InventorySystem.Inventory Base { get; }
        public AmmoBox Ammo { get; }
        public Hand Hand { get; }

        public int ItemsCount => Base.UserInventory.Items.Count;

        public Dictionary<ushort, Item> Items
        {
            get
            {
                Dictionary<ushort, Item> dict = new();
                foreach (var preitem in Base.UserInventory.Items)
                {
                    Item item = Item.Get(preitem.Value);
                    if (item is not null)
                        dict.Add(preitem.Key, item);
                }
                return dict;
            }
            set
            {
                if (value == null)
                {
                    Clear();
                    return;
                }

                Dictionary<ushort, ItemBase> dict = new();
                foreach (var preitem in value)
                {
                    dict.Add(preitem.Key, preitem.Value.Base);
                }
                Base.UserInventory.Items = dict;
                Base.SendItemsNextFrame = true;
            }
        }

        private readonly Player _player;

        internal Inventory(Player player)
        {
            Base = player.ReferenceHub.inventory;
            Ammo = new(player);
            Hand = new(player);
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
        public void Reset(IEnumerable<ItemBase> newItems)
        {
            Clear();

            foreach (ItemBase item in newItems)
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
            => Clear(true);

        public void Clear(bool clearAmmo)
        {
            if (clearAmmo)
            {
                Ammo[AmmoType.Ammo556] = 0;
                Ammo[AmmoType.Ammo762] = 0;
                Ammo[AmmoType.Ammo9] = 0;
                Ammo[AmmoType.Ammo12Gauge] = 0;
                Ammo[AmmoType.Ammo44Cal] = 0;
            }

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