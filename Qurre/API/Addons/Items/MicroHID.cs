using System.Diagnostics.CodeAnalysis;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.MicroHID.Modules;
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
        get => GameBase.EnergyManager.Energy;
        set => GameBase.EnergyManager.ServerSetEnergy(Serial, value);
    }
}