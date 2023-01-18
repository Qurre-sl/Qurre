using InventorySystem.Items.Radio;
using Qurre.API;
using Qurre.API.Addons.Items;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using UnityEngine;

namespace Qurre.Events.Structs
{
    public class CancelUseItemEvent : IBaseEvent
    {
        internal CancelUseItemEvent(Player player, Item item)
        {
            Player = player;
            Item = item;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.CancelUseItem;

        public Player Player { get; }
        public Item Item { get; }
        public bool Allowed { get; set; }
    }

    public class UseItemEvent : IBaseEvent
    {
        internal UseItemEvent(Player player, Item item)
        {
            Player = player;
            Item = item;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.UseItem;

        public Player Player { get; }
        public Item Item { get; }
        public bool Allowed { get; set; }
    }

    public class UsedItemEvent : IBaseEvent
    {
        internal UsedItemEvent(Player player, Item item)
        {
            Player = player;
            Item = item;
        }

        public uint EventId { get; } = PlayerEvents.UsedItem;

        public Player Player { get; }
        public Item Item { get; }
    }

    public class ChangeItemEvent : IBaseEvent
    {
        internal ChangeItemEvent(Player player, Item oldItem, Item newItem)
        {
            Player = player;
            OldItem = oldItem;
            NewItem = newItem;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.ChangeItem;

        public Player Player { get; }
        public Item OldItem { get; }
        public Item NewItem { get; }
        public bool Allowed { get; set; }
    }

    public class UpdateRadioEvent : IBaseEvent
    {
        internal UpdateRadioEvent(Player player, RadioItem radio, RadioStatus range, bool enabled)
        {
            Player = player;
            Radio = Item.SafeGet(radio) as Radio;
            Range = range;
            Enabled = enabled;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.UpdateRadio;

        public Player Player { get; }
        public Radio Radio { get; }
        public RadioStatus Range { get; set; }
        public bool Enabled { get; set; }
        public bool Allowed { get; set; }
    }

    public class UsingRadioEvent : IBaseEvent
    {
        internal UsingRadioEvent(Player player, RadioItem radio, float num)
        {
            Player = player;
            Radio = Item.SafeGet(radio) as Radio;
            Battery = radio._battery * 100;
            Consumption = Time.deltaTime * num;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.UsingRadio;

        public Player Player { get; }
        public Radio Radio { get; }
        public float Battery { get; set; }
        public float Consumption { get; set; }
        public bool Allowed { get; set; }
    }
}