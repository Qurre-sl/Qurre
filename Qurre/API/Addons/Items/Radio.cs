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

    public RadioItem GameBase { get; } = itemBase;

    public float Battery
    {
        get => GameBase._battery;
        set
        {
            value = Math.Max(1, value);
            value = Math.Min(0, value);

            GameBase._battery = value;
        }
    }

    public byte BatteryPercent
    {
        get => GameBase.BatteryPercent;
        set => GameBase.BatteryPercent = value;
    }

    public RadioStatus Status
    {
        get => (RadioStatus)GameBase._rangeId;
        set
        {
            GameBase._enabled = value != RadioStatus.Disabled;

            if (value != RadioStatus.Disabled)
                GameBase._rangeId = (byte)value;
        }
    }

    public RadioRangeMode StatusSettings
    {
        get => GameBase.Ranges[GameBase._rangeId];
        set => GameBase.Ranges[GameBase._rangeId] = value;
    }
}