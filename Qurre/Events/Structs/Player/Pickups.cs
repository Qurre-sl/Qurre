using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Jailbird;
using InventorySystem.Items.Usables.Scp330;
using Qurre.API;
using Qurre.API.Addons.Items;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using System.Collections.Generic;
using static InventorySystem.Items.ThrowableProjectiles.ThrowableItem;

namespace Qurre.Events.Structs
{
    public class PrePickupItemEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.PrePickupItem;

        public Player Player { get; }
        public Pickup Pickup { get; }
        public bool Allowed { get; set; }

        internal PrePickupItemEvent(Player player, Pickup pickup)
        {
            Player = player;
            Pickup = pickup;
            Allowed = true;
        }
    }

    public class PickupItemEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.PickupItem;

        public Player Player { get; }
        public Pickup Pickup { get; }
        public bool Allowed { get; set; }

        internal PickupItemEvent(Player player, Pickup pickup)
        {
            Player = player;
            Pickup = pickup;
            Allowed = true;
        }
    }

    public class PickupAmmoEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.PickupAmmo;

        public Player Player { get; }
        public Pickup Pickup { get; }
        public AmmoPickup Ammo { get; }
        public bool Allowed { get; set; }

        internal PickupAmmoEvent(Player player, Pickup pickup, AmmoPickup ammo)
        {
            Player = player;
            Pickup = pickup;
            Ammo = ammo;
            Allowed = true;
        }
    }

    public class PickupArmorEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.PickupArmor;

        public Player Player { get; }
        public Pickup Pickup { get; }
        public bool Allowed { get; set; }

        internal PickupArmorEvent(Player player, Pickup pickup)
        {
            Player = player;
            Pickup = pickup;
            Allowed = true;
        }
    }

    public class PickupCandyEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.PickupCandy;

        public Player Player { get; }
        public Scp330Bag Bag { get; }
        public List<CandyKindID> List { get; }
        public bool Allowed { get; set; }

        internal PickupCandyEvent(Player player, Scp330Bag bag, List<CandyKindID> list)
        {
            Player = player;
            Bag = bag;
            List = list;
            Allowed = true;
        }
    }

    public class ThrowProjectileEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.ThrowProjectile;

        public Player Player { get; }
        public Throwable Item { get; }
        public ProjectileSettings Settings { get; }
        public bool FullForce { get; }
        public bool Allowed { get; set; }

        internal ThrowProjectileEvent(Player player, Item item, ProjectileSettings settings, bool fullForce)
        {
            Player = player;
            Item = item as Throwable;
            Settings = settings;
            FullForce = fullForce;
            Allowed = true;
        }
    }

    public class DropItemEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.DropItem;

        public Player Player { get; }
        public Item Item { get; }
        public bool Allowed { get; set; }

        internal DropItemEvent(Player player, Item item)
        {
            Player = player;
            Item = item;
            Allowed = true;
        }
    }

    public class DroppedItemEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.DroppedItem;

        public Player Player { get; }
        public Pickup Pickup { get; }

        internal DroppedItemEvent(Player player, Pickup pickup)
        {
            Player = player;
            Pickup = pickup;
        }
    }

    public class DropAmmoEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.DropAmmo;

        public Player Player { get; }
        public AmmoType Type { get; set; }
        public ushort Amount { get; set; }
        public bool Allowed { get; set; }

        internal DropAmmoEvent(Player player, AmmoType type, ushort amount)
        {
            Player = player;
            Type = type;
            Amount = amount;
            Allowed = true;
        }
    }

    public class JailbirdTriggerEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.JailbirdTrigger;

        Item _item;

        public Player Player { get; }
        public Item Item
        {
            get
            {
                _item ??= Item.Get(JailbirdBase);
                return _item;
            }
        }
        public JailbirdItem JailbirdBase { get; }
        public JailbirdMessageType Message { get; set; }
        public bool Allowed { get; set; }

        internal JailbirdTriggerEvent(Player player, JailbirdItem @base, JailbirdMessageType message)
        {
            Player = player;
            JailbirdBase = @base;
            Message = message;
            Allowed = true;
        }
    }
}