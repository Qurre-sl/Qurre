using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Pickups;
using JetBrains.Annotations;
using Qurre.API.Classification.Structs;
using Qurre.API.Objects;

namespace Qurre.API.Classification.Player;

[PublicAPI]
public sealed class Inventory
{
    private readonly Controllers.Player _player;

    internal Inventory(Controllers.Player player)
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

    public Dictionary<ushort, ItemBase> Items
    {
        get => Base.UserInventory.Items;
        set
        {
            Dictionary<ushort, ItemBase> dict = [];

            foreach (var preItem in value)
                dict.Add(preItem.Key, preItem.Value);

            Base.UserInventory.Items = dict;
            Base.SendItemsNextFrame = true;
        }
    }

    public bool HasItem(ItemType item)
    {
        return Base.UserInventory.Items.Any(tempItem => tempItem.Value.ItemTypeId == item);
    }

    public void Reset(IEnumerable<ItemBase> newItems)
    {
        Clear();

        foreach (var item in newItems)
            AddItem(item);
    }

    public void Reset(IEnumerable<ItemType> newItems)
    {
        Clear();

        foreach (var type in newItems)
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

    public void SelectItem(ItemBase item)
    {
        SelectItem(item.ItemSerial);
    }

    public void DropItem(ushort serial)
    {
        Base.ServerDropItem(serial);
    }

    public void DropItem(ItemBase item)
    {
        Base.ServerDropItem(item.ItemSerial);
    }

    public ItemBase? AddItem(ItemBase itemBase)
    {
        if (!itemBase)
            return null;

        if (!itemBase.PickupDropModel)
            return null;

        Base.UserInventory.Items[itemBase.PickupDropModel.NetworkInfo.Serial] = itemBase;
        itemBase.OnAdded(itemBase.PickupDropModel);

        if (itemBase is Firearm firearm)
            SetupFirearmAttachments(_player.ReferenceHub, firearm);

        Base.SendItemsNextFrame = true;

        return itemBase;
    }

    public void AddItem(ItemBase item, uint amount)
    {
        if (amount == 0)
            return;

        for (uint i = 0; i < amount; i++)
            AddItem(item);
    }

    public ItemBase? AddItem(ItemType itemType)
    {
        return LabApi.Features.Wrappers.Player.Get(_player.ReferenceHub).AddItem(itemType)?.Base;
    }

    public void AddItem(ItemType itemType, uint amount)
    {
        if (amount == 0)
            return;

        for (uint i = 0; i < amount; i++)
            AddItem(itemType);
    }

    public void AddItem(IEnumerable<ItemBase> items)
    {
        foreach (var item in items)
            AddItem(item);
    }

    public void RemoveItem(ushort serial, ItemPickupBase itemPickupBase)
    {
        Base.ServerRemoveItem(serial, itemPickupBase);
    }

    public void RemoveItem(ItemPickupBase pickup)
    {
        Base.ServerRemoveItem(pickup.Info.Serial, pickup);
    }

    public void RemoveItem(ItemBase item)
    {
        if (!item.PickupDropModel)
            return;

        Base.ServerRemoveItem(item.ItemSerial, item.PickupDropModel);
    }


    private static void SetupFirearmAttachments(ReferenceHub referenceHub, Firearm firearm)
    {
        if (!AttachmentsServerHandler.PlayerPreferences.TryGetValue(referenceHub, out var itemPreferences) ||
            !itemPreferences.TryGetValue(firearm.ItemTypeId, out var attachmentsCode))
            attachmentsCode = 0U;

        firearm.ApplyAttachmentsCode(attachmentsCode, true);
        firearm.ServerResendAttachmentCode();
        AttachmentsServerHandler.ServerApplyPreference(referenceHub, firearm.ItemTypeId, attachmentsCode);
    }
}