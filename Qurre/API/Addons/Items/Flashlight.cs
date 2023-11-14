using InventorySystem.Items.ToggleableLights;
using InventorySystem.Items.ToggleableLights.Flashlight;
using Qurre.API.Controllers;
using Utils.Networking;

namespace Qurre.API.Addons.Items
{
    public sealed class Flashlight : Item
    {
        private const ItemType FlashlightItemType = ItemType.Flashlight;

        public new FlashlightItem Base { get; }

        public bool Active
        {
            get => Base.IsEmittingLight;
            set
            {
                Base.IsEmittingLight = value;
                NetworkUtils.SendToAuthenticated(new FlashlightNetworkHandler.FlashlightMessage(base.Serial, value));
            }
        }

        public Flashlight(FlashlightItem itemBase) : base(itemBase)
        {
            Base = itemBase;
        }

        public Flashlight() : this((FlashlightItem)FlashlightItemType.CreateItemInstance())
        {
        }
    }
}