using InventorySystem.Items.Flashlight;
using Qurre.API.Controllers;
using Utils.Networking;

namespace Qurre.API.Addons.Items
{
    public sealed class Flashlight : Item
    {
        private const ItemType FlashlightItemType = ItemType.Flashlight;

        public Flashlight(FlashlightItem itemBase) : base(itemBase)
            => Base = itemBase;

        public Flashlight() : this((FlashlightItem)FlashlightItemType.CreateItemInstance()) { }

        public new FlashlightItem Base { get; }

        public bool Active
        {
            get => Base.IsEmittingLight;
            set
            {
                Base.IsEmittingLight = value;
                new FlashlightNetworkHandler.FlashlightMessage(Serial, value).SendToAuthenticated();
            }
        }
    }
}