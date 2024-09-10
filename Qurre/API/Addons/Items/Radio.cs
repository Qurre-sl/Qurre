using System;
using InventorySystem.Items.Radio;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using Qurre.API.Objects;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public sealed class Radio(RadioItem itemBase) : Item(itemBase)
{
    private const ItemType RadioItemType = ItemType.Radio;

    public Radio() : this((RadioItem)RadioItemType.CreateItemInstance())
    {
    }

    public new RadioItem Base { get; } = itemBase;

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
}