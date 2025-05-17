using System.Collections.Generic;
using InventorySystem.Items;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Jailbird;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables.Scp330;
using JetBrains.Annotations;
using Qurre.API.Entities.Characters;
using Qurre.API.Enums;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

using static ThrowableItem;

[PublicAPI]
public class PrePickupItemEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.PrePickupItem;

    internal PrePickupItemEvent(Player player, ItemPickupBase pickup)
    {
        Player = player;
        Pickup = pickup;
        IsAllowed = true;
    }

    public Player Player { get; }
    public ItemPickupBase Pickup { get; }
    public bool IsAllowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class PickupItemEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.PickupItem;

    internal PickupItemEvent(Player player, ItemPickupBase pickup)
    {
        Player = player;
        Pickup = pickup;
        IsAllowed = true;
    }

    public Player Player { get; }
    public ItemPickupBase Pickup { get; }
    public bool IsAllowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class PickupAmmoEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.PickupAmmo;

    internal PickupAmmoEvent(Player player, ItemPickupBase pickup, AmmoPickup ammo)
    {
        Player = player;
        Pickup = pickup;
        Ammo = ammo;
        IsAllowed = true;
    }

    public Player Player { get; }
    public ItemPickupBase Pickup { get; }
    public AmmoPickup Ammo { get; }
    public bool IsAllowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class PickupArmorEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.PickupArmor;

    internal PickupArmorEvent(Player player, ItemPickupBase pickup)
    {
        Player = player;
        Pickup = pickup;
        IsAllowed = true;
    }

    public Player Player { get; }
    public ItemPickupBase Pickup { get; }
    public bool IsAllowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class PickupCandyEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.PickupCandy;

    internal PickupCandyEvent(Player player, Scp330Bag bag, List<CandyKindID> list)
    {
        Player = player;
        Bag = bag;
        List = list;
        IsAllowed = true;
    }

    public Player Player { get; }
    public Scp330Bag Bag { get; }
    public List<CandyKindID> List { get; }
    public bool IsAllowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class ThrowProjectileEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.ThrowProjectile;

    internal ThrowProjectileEvent(Player player, ThrowableItem item, ProjectileSettings settings, bool fullForce)
    {
        Player = player;
        Item = item;
        Settings = settings;
        FullForce = fullForce;
        IsAllowed = true;
    }

    public Player Player { get; }
    public ThrowableItem Item { get; }
    public ProjectileSettings Settings { get; }
    public bool FullForce { get; }
    public bool IsAllowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class DropItemEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.DropItem;

    internal DropItemEvent(Player player, ItemBase item)
    {
        Player = player;
        Item = item;
        IsAllowed = true;
    }

    public Player Player { get; }
    public ItemBase Item { get; }
    public bool IsAllowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class DroppedItemEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.DroppedItem;

    internal DroppedItemEvent(Player player, ItemPickupBase pickup)
    {
        Player = player;
        Pickup = pickup;
    }

    public Player Player { get; }
    public ItemPickupBase Pickup { get; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class DropAmmoEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.DropAmmo;

    internal DropAmmoEvent(Player player, AmmoTypes ammoType, ushort amount)
    {
        Player = player;
        AmmoType = ammoType;
        Amount = amount;
        IsAllowed = true;
    }

    public Player Player { get; }
    public AmmoTypes AmmoType { get; set; }
    public ushort Amount { get; set; }
    public bool IsAllowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class JailbirdTriggerEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.JailbirdTrigger;

    internal JailbirdTriggerEvent(Player player, JailbirdItem jailbirdBase, JailbirdMessageType message)
    {
        Player = player;
        Item = jailbirdBase;
        Message = message;
        JailbirdBase = jailbirdBase;
        IsAllowed = true;
    }

    public Player Player { get; }
    public JailbirdItem Item { get; }

    public JailbirdItem JailbirdBase { get; }
    public JailbirdMessageType Message { get; set; }
    public bool IsAllowed { get; set; }

    public uint EventId { get; } = EventID;
}