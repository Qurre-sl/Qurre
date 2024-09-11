using InventorySystem.Items.Firearms.Ammo;
using JetBrains.Annotations;
using Qurre.API.Controllers;
using Qurre.API.Objects;

namespace Qurre.API.Addons.Items;

[PublicAPI]
public sealed class Ammo(AmmoItem itemBase) : Item(itemBase)
{
    public Ammo(ItemType itemType) : this((AmmoItem)itemType.CreateItemInstance())
    {
    }

    public Ammo(AmmoType ammoType) : this(ammoType.GetItemType())
    {
    }

    public int UnitPrice
    {
        get => Base.UnitPrice;
        set => Base.UnitPrice = value;
    }

    public new AmmoItem Base { get; } = itemBase;
}