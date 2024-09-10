using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Pickups;
using JetBrains.Annotations;
using Qurre.API.Addons.Items;
using Qurre.API.Classification.Structs;
using Qurre.API.Controllers;
using Qurre.API.Objects;

namespace Qurre.API.Classification.Player;

[PublicAPI]
public sealed class Inventory
{
    private readonly API.Player _player;

    internal Inventory(API.Player player)
    {
        Base = player.ReferenceHub.inventory;
        Ammo = new AmmoBox(player);
        Hand = new Hand(player);
        _player = player;
    }

    public InventorySystem.Inventory Base { get; }
    public AmmoBox Ammo { get; }
    public Hand Hand { get; }

    public int ItemsCount => Base.UserInventory.Items.Count;

    public Dictionary<ushort, Item> Items
    {
        get
        {
            Dictionary<ushort, Item> dict = [];
            foreach (var preItem in Base.UserInventory.Items)
            {
                Item? item = Item.Get(preItem.Value);
                if (item is not null)
                    dict.Add(preItem.Key, item);
            }

            return dict;
        }
        set
        {
            Dictionary<ushort, ItemBase> dict = [];

            foreach (var preItem in value)
                dict.Add(preItem.Key, preItem.Value.Base);

            Base.UserInventory.Items = dict;
            Base.SendItemsNextFrame = true;
        }
    }

    public bool HasItem(ItemType item)
    {
        return Base.UserInventory.Items.Any(tempItem => tempItem.Value.ItemTypeId == item);
    }

    public void Reset(IEnumerable<Item> newItems)
    {
        Clear();

        foreach (Item item in newItems)
            AddItem(item);
    }

    public void Reset(IEnumerable<ItemBase> newItems)
    {
        Clear();

        foreach (ItemBase item in newItems)
            AddItem(item);
    }

    public void Reset(IEnumerable<ItemType> newItems)
    {
        Clear();

        foreach (ItemType type in newItems)
            AddItem(type);
    }

    public void Clear()
    {
        Clear(true);
    }

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

        while (Base.UserInventory.Items.Count != 0)
            Base.ServerRemoveItem(Base.UserInventory.Items.ElementAt(0).Key, null);
    }

    public void DropAllAmmo()
    {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var item in Base.UserInventory.ReserveAmmo)
            if (item.Value != 0)
                Base.ServerDropAmmo(item.Key, ushort.MaxValue);
    }

    public void DropAllItems()
    {
        while (Base.UserInventory.Items.Count != 0)
            Base.ServerDropItem(Base.UserInventory.Items.First().Key);
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
        SelectItem(item.Serial);
    }

    public void DropItem(ushort serial)
    {
        Base.ServerDropItem(serial);
    }

    public void DropItem(Item item)
    {
        Base.ServerDropItem(item.Serial);
    }

    public Item? AddItem(ItemBase itemBase)
    {
        if (itemBase == null)
            return null;

        if (itemBase.PickupDropModel == null)
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
        AddItem(item.Base);
    }

    public void AddItem(Item item, uint amount)
    {
        if (amount == 0)
            return;

        for (uint i = 0; i < amount; i++)
            AddItem(item.Base);
    }

    public Item? AddItem(ItemType itemType)
    {
        Item? item = Item.Get(Base.ServerAddItem(itemType));

        if (item is not Gun gun)
            return item;

        if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(_player.ReferenceHub, out var dict)
            && dict.TryGetValue(itemType, out uint code))
            gun.Base.ApplyAttachmentsCode(code, true);

        FirearmStatusFlags status = FirearmStatusFlags.MagazineInserted;
        if (gun.Base.HasAdvantageFlag(AttachmentDescriptiveAdvantages.Flashlight))
            status |= FirearmStatusFlags.FlashlightEnabled;

        gun.Base.Status = new FirearmStatus(gun.MaxAmmo, status, gun.Base.GetCurrentAttachmentsCode());

        return item;
    }

    public void AddItem(ItemType itemType, uint amount)
    {
        if (amount == 0)
            return;

        for (uint i = 0; i < amount; i++)
            AddItem(itemType);
    }

    public void AddItem(IEnumerable<Item> items)
    {
        foreach (Item item in items)
            AddItem(item.Base);
    }

    public void RemoveItem(ushort serial, ItemPickupBase itemPickupBase)
    {
        Base.ServerRemoveItem(serial, itemPickupBase);
    }

    public void RemoveItem(Pickup pickup)
    {
        Base.ServerRemoveItem(pickup.Serial, pickup.Base);
    }

    public void RemoveItem(Item item)
    {
        if (item.Pickup == null)
            return;

        Base.ServerRemoveItem(item.Serial, item.Pickup.Base);
    }
}