using InventorySystem.Items.Usables;
using Qurre.API.Controllers;

namespace Qurre.API.Addons.tems
{
    public sealed class Usable : Item
    {
        public new UsableItem Base { get; }

        public bool Equippable => Base.AllowEquip;
        public bool Holsterable => Base.AllowHolster;

        public new float Weight
        {
            get => Base._weight;
            set => Base._weight = value;
        }

        public float UseTime
        {
            get => Base.UseTime;
            set => Base.UseTime = value;
        }

        public float MaxCancellableTime
        {
            get => Base.MaxCancellableTime;
            set => Base.MaxCancellableTime = value;
        }

        public float RemainingCooldown
        {
            get => Base.RemainingCooldown;
            set => Base.RemainingCooldown = value;
        }

        public Usable(UsableItem itemBase) : base(itemBase)
        {
            Base = itemBase;
        }

        public Usable(ItemType type) : this((UsableItem)type.CreateItemInstance())
        {
        }
    }
}
