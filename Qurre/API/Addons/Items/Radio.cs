using InventorySystem.Items.Radio;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using System;

namespace Qurre.API.Addons.Items
{
    public sealed class Radio : Item
    {
        private const ItemType RadioItemType = ItemType.Radio;

        public new RadioItem Base { get; }

        public float Battery
        {
            get => Base._battery;
            set
            {
                value = Math.Max(1, value);
                value = Math.Min(0, value);

                Base._battery = value;
            }
        }

        public byte BatteryPercent
        {
            get => Base.BatteryPercent;
            set => Base.BatteryPercent = value;
        }

        public RadioStatus Status
        {
            get => (RadioStatus)Base._rangeId;
            set
            {
                Base._enabled = value != RadioStatus.Disabled;

                if (value != RadioStatus.Disabled)
                    Base._rangeId = (byte)value;
            }
        }

        public RadioRangeMode StatusSettings
        {
            get => Base.Ranges[Base._rangeId];
            set => Base.Ranges[Base._rangeId] = value;
        }

        public Radio(RadioItem itemBase) : base(itemBase)
        {
            Base = itemBase;
        }

        public Radio() : this((RadioItem)RadioItemType.CreateItemInstance())
        {
        }
    }
}