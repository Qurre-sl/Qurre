using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Radio;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.ToggleableLights.Flashlight;
using InventorySystem.Items.Usables;
using JetBrains.Annotations;
using Mirror;
using Qurre.API.Addons.Items;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API.Controllers;

[PublicAPI]
public class Item
{
    internal static readonly Dictionary<ItemBase, Item> BaseToItem = [];

    private string _tag = string.Empty;

    public Item(ItemBase itemBase)
    {
        Base = itemBase;
        Serial = Base.OwnerInventory.UserInventory.Items.FirstOrDefault(i => i.Value == Base).Key;
        if (Serial == 0)
            Serial = ItemSerialGenerator.GenerateNext();

        BaseToItem.Add(itemBase, this);
    }

    public Item(ItemType type)
        : this(Server.InventoryHost.CreateItemInstance(new ItemIdentifier(type, ItemSerialGenerator.GenerateNext()),
            true))
    {
    }

    public ItemBase Base { get; }

    public ItemCategory Category => Base.Category;
    public float Weight => Base.Weight;

    public ItemType Type
        => Base.ItemTypeId;

    public Player Owner => Base.Owner != null ? Base.Owner.GetPlayer() ?? Server.Host : Server.Host;

    public Pickup? Pickup => Pickup.BaseToItem.GetValueOrDefault(Base.PickupDropModel);

    public string Tag
    {
        get => _tag;
        set
        {
            if (string.IsNullOrEmpty(value))
                value = string.Empty;

            _tag = value;
        }
    }

    public ushort Serial
    {
        get => Base.ItemSerial;
        internal set
        {
            if (value == 0) value = ItemSerialGenerator.GenerateNext();
            if (Base == null || Base.PickupDropModel == null)
                return;
            Base.PickupDropModel.Info.Serial = value;
            Base.PickupDropModel.NetworkInfo = Base.PickupDropModel.Info;
        }
    }

    internal static Item? SafeGet(ItemBase itemBase)
    {
        try
        {
            return Get(itemBase);
        }
        catch (Exception e)
        {
            Log.Debug(e);
            return null;
        }
    }

    internal static Item UnsafeGet(ItemBase itemBase)
    {
        if (BaseToItem.TryGetValue(itemBase, out Item? value))
            return value;

        return itemBase switch
        {
            Firearm gun => new Gun(gun),
            KeycardItem card => new Keycard(card),
            UsableItem usable =>
                //if (usable is Scp330Bag bag)
                //    return new Scp330(bag);
                new Usable(usable),
            RadioItem radio => new Radio(radio),
            MicroHIDItem hid => new MicroHID(hid),
            BodyArmor armor => new Armor(armor),
            AmmoItem ammo => new Ammo(ammo),
            FlashlightItem flashlight => new Flashlight(flashlight),
            ThrowableItem throwable => throwable.Projectile switch
            {
                FlashbangGrenade _ => new GrenadeFlash(throwable),
                ExplosionGrenade _ => new GrenadeFrag(throwable),
                _ => new Throwable(throwable)
            },
            _ => new Item(itemBase)
        };
    }

    public static Item? Get(ItemBase itemBase)
    {
        return itemBase == null ? null : UnsafeGet(itemBase);
    }

    public static Item? Get(ushort serial)
    {
        return Object.FindObjectsOfType<ItemBase>().TryFind(out var item, x => x.ItemSerial == serial)
            ? SafeGet(item)
            : null;
    }

    public void Give(Player player)
    {
        player.Inventory.AddItem(Base);
    }

    public virtual Pickup Spawn(Vector3 position, Quaternion rotation = default, Vector3 scale = default)
    {
        ItemPickupBase ipb = Object.Instantiate(Base.PickupDropModel, position, rotation);

        ipb.Info.ItemId = Type;
        ipb.Info.WeightKg = Weight;
        ipb.NetworkInfo = ipb.Info;

        ipb.Position = position;
        ipb.Rotation = rotation;

        if (ipb is FirearmPickup firearmPickup) firearmPickup.NetworkStatus = firearmPickup.Status;

        NetworkServer.Spawn(ipb.gameObject);

        Pickup? pickup = Pickup.Get(ipb);

        if (pickup is null)
            throw new NullReferenceException("Pickup could not be found");

        pickup.Scale = scale == default ? Vector3.one : scale;
        return pickup;
    }

    public override bool Equals(object? obj)
    {
        return obj is Item item && Serial == item.Serial;
    }

    public override int GetHashCode()
    {
        return -29014143 + Serial.GetHashCode();
    }

    public static bool operator ==(Item? a, Item? b)
    {
        return a?.Serial == b?.Serial;
    }

    public static bool operator !=(Item a, Item b)
    {
        return !(a == b);
    }
}