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

    public MicroHIDItem GameBase { get; } = itemBase;

    public float Energy
    {
        get => GameBase.RemainingEnergy;
        set => GameBase.RemainingEnergy = value;
    }

    /// <summary>
    ///     0 - 255
    /// </summary>
    public byte EnergyPercent
    {
        get => GameBase.EnergyToByte;
        set => GameBase.RemainingEnergy = value / 225f;
    }

    public HidState State
    {
        get => GameBase.State;
        set => GameBase.State = value;
    }

    public void Fire()
    {
        GameBase.UserInput = HidUserInput.Fire;
        State = HidState.Firing;
    }
}