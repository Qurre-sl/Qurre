using System.Diagnostics.CodeAnalysis;
using InventorySystem.Items.MicroHID;
using JetBrains.Annotations;
using Qurre.API.Controllers;

namespace Qurre.API.Addons.Items;

[PublicAPI]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class MicroHID(MicroHIDItem itemBase) : Item(itemBase)
{
    private const ItemType MicroHIDItemType = ItemType.MicroHID;

    public MicroHID() : this((MicroHIDItem)MicroHIDItemType.CreateItemInstance())
    {
    }

    public new MicroHIDItem Base { get; } = itemBase;

    public float Energy
    {
        get => Base.RemainingEnergy;
        set => Base.RemainingEnergy = value;
    }

    /// <summary>
    ///     0 - 255
    /// </summary>
    public byte EnergyPercent
    {
        get => Base.EnergyToByte;
        set => Base.RemainingEnergy = value / 225f;
    }

    public HidState State
    {
        get => Base.State;
        set => Base.State = value;
    }

    public void Fire()
    {
        Base.UserInput = HidUserInput.Fire;
        State = HidState.Firing;
    }
}