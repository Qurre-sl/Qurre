using InventorySystem.Items.Radio;
using Qurre.API;
using Qurre.API.Addons.Items;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Events.Structs
{
    public class CancelUseItemEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.CancelUseItem;

        public Player Player { get; }
        public Item Item { get; }
        public bool Allowed { get; set; }

        internal CancelUseItemEvent(Player player, Item item)
        {
            Player = player;
            Item = item;
            Allowed = true;
        }
    }

    public class UseItemEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.UseItem;

        public Player Player { get; }
        public Item Item { get; }
        public bool Allowed { get; set; }

        internal UseItemEvent(Player player, Item item)
        {
            Player = player;
            Item = item;
            Allowed = true;
        }
    }

    public class UsedItemEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.UsedItem;

        public Player Player { get; }
        public Item Item { get; }

        internal UsedItemEvent(Player player, Item item)
        {
            Player = player;
            Item = item;
        }
    }

    public class ChangeItemEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.ChangeItem;

        public Player Player { get; }
        public Item OldItem { get; }
        public Item NewItem { get; }
        public bool Allowed { get; set; }

        internal ChangeItemEvent(Player player, Item oldItem, Item newItem)
        {
            Player = player;
            OldItem = oldItem;
            NewItem = newItem;
            Allowed = true;
        }
    }

    public class UpdateRadioEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.UpdateRadio;

        public Player Player { get; }
        public Radio Radio { get; }
        public RadioStatus Range { get; set; }
        public bool Enabled { get; set; }
        public bool Allowed { get; set; }

        internal UpdateRadioEvent(Player player, RadioItem radio, RadioStatus range, bool enabled)
        {
            Player = player;
            Radio = Item.SafeGet(radio) as Radio;
            Range = range;
            Enabled = enabled;
            Allowed = true;
        }
    }

    public class UsingRadioEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.UsingRadio;

        public Player Player { get; }
        public Radio Radio { get; }
        public float Battery { get; set; }
        public float Consumption { get; set; }
        public bool Allowed { get; set; }

        internal UsingRadioEvent(Player player, RadioItem radio, float num)
        {
            Player = player ?? Server.Host;
            Radio = Item.SafeGet(radio) as Radio;
            Battery = radio._battery * 100;
            Consumption = UnityEngine.Time.deltaTime * (num / 60 / 100) * 100;
            Allowed = true;
        }
    }
}