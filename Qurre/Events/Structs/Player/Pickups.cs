using System.Collections.Generic;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Jailbird;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables.Scp330;
using JetBrains.Annotations;
using Qurre.API.Entities;
using Qurre.API.Entities.Characters;
using Qurre.API.Entities.Items;
using Qurre.API.Entities.Items.Implementations;
using Qurre.API.Enums;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

using static ThrowableItem;

[PublicAPI]
public class PrePickupItemEvent : ICancellableEvent
{
    internal PrePickupItemEvent(Player player, IPickup pickup)
    {
        Player = player;
        Pickup = pickup;
        IsAllowed = true;
    }

    public Player Player { get; }
    public IPickup Pickup { get; }
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.PrePickupItem;
}

[PublicAPI]
public class PickupItemEvent : ICancellableEvent
{
    internal PickupItemEvent(Player player, IPickup pickup)
    {
        Player = player;
        Pickup = pickup;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.PickupItem;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IPickup Pickup { get; }
}

[PublicAPI]
public class PickupAmmoEvent : ICancellableEvent
{
    internal PickupAmmoEvent(Player player, IPickup pickup, AmmoPickup ammo)
    {
        Player = player;
        Pickup = pickup;
        Ammo = ammo;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.PickupAmmo;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IPickup Pickup { get; }
    public AmmoPickup Ammo { get; }
}

[PublicAPI]
public class PickupArmorEvent : ICancellableEvent
{
    internal PickupArmorEvent(Player player, IPickup pickup)
    {
        Player = player;
        Pickup = pickup;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.PickupArmor;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IPickup Pickup { get; }
}

[PublicAPI]
public class PickupCandyEvent : ICancellableEvent
{
    internal PickupCandyEvent(Player player, Scp330Bag bag, List<CandyKindID> list)
    {
        Player = player;
        Bag = bag;
        List = list;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.PickupCandy;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public Scp330Bag Bag { get; }
    public List<CandyKindID> List { get; }
}

[PublicAPI]
public class ThrowProjectileEvent : ICancellableEvent
{
    internal ThrowProjectileEvent(Player player, Throwable item, ProjectileSettings settings, bool fullForce)
    {
        Player = player;
        Item = item;
        Settings = settings;
        FullForce = fullForce;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.ThrowProjectile;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IThrowable Item { get; }
    public ProjectileSettings Settings { get; }
    public bool FullForce { get; }
}

[PublicAPI]
public class DropItemEvent : ICancellableEvent
{
    internal DropItemEvent(Player player, IItem item)
    {
        Player = player;
        Item = item;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.DropItem;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IItem Item { get; }
}

[PublicAPI]
public class DroppedItemEvent : IBaseEvent
{
    internal DroppedItemEvent(Player player, IPickup pickup)
    {
        Player = player;
        Pickup = pickup;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.DroppedItem;

    public Player Player { get; }
    public IPickup Pickup { get; }
}

[PublicAPI]
public class DropAmmoEvent : ICancellableEvent
{
    internal DropAmmoEvent(Player player, AmmoTypes ammoType, ushort amount)
    {
        Player = player;
        AmmoType = ammoType;
        Amount = amount;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.DropAmmo;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public AmmoTypes AmmoType { get; set; }
    public ushort Amount { get; set; }
}

[PublicAPI]
public class JailbirdTriggerEvent : ICancellableEvent
{
    internal JailbirdTriggerEvent(Player player, JailbirdItem jailbirdBase, JailbirdMessageType message)
    {
        Player = player;
        Item = EntityManager.GetOrException<IItem>(jailbirdBase);
        Message = message;
        JailbirdBase = jailbirdBase;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.JailbirdTrigger;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IItem Item { get; }

    public JailbirdItem JailbirdBase { get; }
    public JailbirdMessageType Message { get; set; }
}