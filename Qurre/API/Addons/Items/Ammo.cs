using InventorySystem.Items.Firearms.Ammo;
using Qurre.API.Objects;
using Qurre.API.Controllers;

namespace Qurre.API.Addons.Items
{
    public sealed class Ammo : Item
    {
        public int UnitPrice
        {
            get => Base.UnitPrice;
            set => Base.UnitPrice = value;
        }

        public new AmmoItem Base { get; }

        public Ammo(AmmoItem itemBase) : base(itemBase)
        {
            Base = itemBase;
        }

        public Ammo(ItemType itemType) : this((AmmoItem)itemType.CreateItemInstance())
        {
        }

        public Ammo(AmmoType ammoType) : this(ammoType.GetItemType())
        {
        }
    }
}